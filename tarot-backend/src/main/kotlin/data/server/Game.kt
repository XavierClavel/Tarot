package xclavel.data.server

import com.sun.org.apache.xpath.internal.operations.Bool
import kotlinx.coroutines.delay
import org.koin.core.component.get
import org.koin.java.KoinJavaComponent.inject
import xclavel.InvalidAction
import xclavel.data.tarot.Card
import xclavel.data.tarot.CardsDealing
import xclavel.data.tarot.Color
import xclavel.data.tarot.data.tarot.Bid
import xclavel.services.services.TarotService
import xclavel.utils.logger

class Game(val lobby: Lobby) {
    val tarotService by inject<TarotService>(TarotService::class.java)
    val hands = HashMap<Player, MutableList<Card>>()
    val points = mutableListOf<Card>()
    val bids = HashMap<Player, Bid>()
    val currentLevee = mutableListOf<Card>()
    var calledKing: Color? = null
    var currentPlayer: Player? = null
    var playersOrder = mutableListOf<Player>()
    var turn = 1
    var cardsDealing: CardsDealing? = null
    val playersReady = mutableSetOf<Player>()

    val attackers = mutableListOf<Player>()
    var awaitingDog = false


    suspend fun onPlayerReady(player: Player) {
        playersReady.add(player)
        if (playersReady.size !=lobby.players.size ) return
        playersOrder = lobby.players.values.toMutableList()
        currentPlayer = playersOrder[0]
        lobby.broadcast(PlayerTurn(currentPlayer!!.username))
    }


    suspend fun receiveBid(player: Player, bid: Bid) {
        logger.info {"Received bid $bid for player ${player.username}"}
        if (bids.containsKey(player)) throw InvalidAction("Player has already made a bid")
        if (player != currentPlayer) throw InvalidAction("Not your turn")
        if (!isBidValid(bid)) throw InvalidAction("Invalid bid")
        bids[player] = bid;
        lobby.broadcast(BidMade(bid))
        if (bids.size != lobby.players.size) {
            switchToNextPlayer()
            return
        }

        logger.info {"Bids over"}
        if (bids.values.all { it == Bid.PASSE }) { //fausse donne
            logger.info { "fausse donne" }
        } else {
            val highestBidder = bids.maxBy { it.value.ordinal }.key
            attackers.add(highestBidder)
            lobby.broadcast(BidResult(highestBidder.username, bids[highestBidder]!!))
            logger.info {"Bid won by ${highestBidder.username} with ${bids[highestBidder]}"}
            currentPlayer = highestBidder
            lobby.broadcast(DogReveal(currentPlayer!!.username, cardsDealing!!.dog))
            awaitingDog = true
        }
    }

    suspend fun receiveDog(cardIds: List<Int>, player: Player) {
        if (player != currentPlayer) throw InvalidAction("Not your turn")
        if (!awaitingDog) throw InvalidAction("Not the right time")
        awaitingDog = false
        val cards = cardIds.map {Card.fromId(it, player)}
        if (cards.any { it.isOudler() || it.isRoi() }) throw InvalidAction("Invalid dog")
         cards.forEach { card ->
            card.scoredBy = player
            points.add(card)
        }
        lobby.broadcast(PlayerTurn(currentPlayer!!.username))
    }

    fun isGameOver(): Boolean =
        hands.all { it.value.isEmpty() }

    suspend fun onGameOver() {
        val scoredPoints = points.filter { it.scoredBy in attackers }
        val oudlersCount = tarotService.countOudlers(scoredPoints)
        val threshold = tarotService.getWinThreshold(oudlersCount)
        val pointsAmount = tarotService.countPoints(scoredPoints, threshold)
        val score = tarotService.calculateScore(pointsAmount, threshold, bids[attackers[0]]!!, lobby.players.size)
        val result = mutableMapOf<Player, Int>()
        val attackerMultiplier = tarotService.getPlayerAmountMultiplier(lobby.players.size)

         if (attackers.size == 1) {
            result.put(attackers[0], score * attackerMultiplier)
            lobby.players.values
                .filterNot { it in attackers }
                .forEach {
                    result.put(it, -score)
                }
        } else {
             result.put(attackers[0], score * attackerMultiplier * 2/3)
             result.put(attackers[1], score * attackerMultiplier * 1/3)
             lobby.players.values
                 .filterNot { it in attackers }
                 .forEach {
                     result.put(it, -score)
                 }
        }
        lobby.addGameResult(result)

        lobby.broadcast(GameOver(score > 0, pointsAmount, result.mapKeys { it.key.username }))
    }

    fun isBidValid(bid: Bid): Boolean {
        return if (bid == Bid.PASSE) true
        else bids.values.none { it.ordinal >= bid.ordinal }
    }

    fun dealCards() {
        lobby.deck.cut()
        logger.info {lobby.players}
        cardsDealing = lobby.deck.deal(lobby.players.values.toList())
        logger.info {cardsDealing}
        cardsDealing!!.hands.forEach { hand ->
            hands.put(hand.key, hand.value.map { Card.fromId(it, hand.key) }.toMutableList())
        }
    }

    suspend fun playCard(player: Player, cardIndex: Int) {
        val card = Card.fromId(cardIndex, player)
        if (player != currentPlayer) {
            throw InvalidAction("Not your turn")
        }
        if (!cardsDealing!!.hands[player]!!.contains(cardIndex)) {
            throw InvalidAction("Not your card")
        }
        if (!hands[player]!!.any { it.id == cardIndex }) {
            throw InvalidAction("Card already used")
        }

        tarotService.checkActionValidity(card, currentLevee, hands[player]!!, turn, calledKing)

        currentLevee.add(card)
        hands[player]!!.removeIf{ it.id == cardIndex}
        lobby.broadcast(CardPlayed(cardIndex))

        if (!isRoundComplete()) {
            switchToNextPlayer()
            return
        }

        val bestCard = tarotService.findBestCard(currentLevee)
        logger.info {bestCard}
        lobby.broadcast(TurnWon(bestCard.owner!!.username))
        winTurn(bestCard.owner!!)
        delay(3000L)

        if (isGameOver()) {
            onGameOver()
        } else {
            lobby.broadcast(PlayerTurn(bestCard.owner!!.username))
        }

    }

    fun winTurn(player: Player) {
        currentLevee.forEach { card ->
            card.scoredAt = turn
            card.scoredBy = player
            points.add(card)
        }
        currentLevee.clear()
        turn++
        currentPlayer = player
    }

    private fun isRoundComplete(): Boolean = currentLevee.size == playersOrder.size

    private suspend fun switchToNextPlayer() {
        val playerIndex = playersOrder.indexOf(currentPlayer)
        currentPlayer = if (playerIndex == playersOrder.size - 1) {
            playersOrder[0]
        } else {
            playersOrder[playerIndex + 1]
        }
        lobby.broadcast(PlayerTurn(currentPlayer!!.username))
    }



}