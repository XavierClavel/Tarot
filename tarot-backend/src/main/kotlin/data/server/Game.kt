package xclavel.data.server

import kotlinx.coroutines.delay
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
    val points = HashMap<Player, MutableSet<Card>>()
    val bids = HashMap<Player, Bid>()
    val currentLevee = mutableListOf<Card>()
    var calledKing: Color? = null
    var currentPlayer: Player? = null
    var playersOrder = mutableListOf<Player>()
    var turn = 1
    var cardsDealing: CardsDealing? = null
    val playersReady = mutableSetOf<Player>()


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
        if (bids.size == lobby.players.size) {
            logger.info {"Bids over"}
            if (bids.values.all { it == Bid.PASSE }) { //fausse donne
                logger.info { "fausse donne" }
            } else {
                val highestBidder = bids.maxBy { it.value.ordinal }.key
                lobby.broadcast(BidResult(highestBidder.username, bids[highestBidder]!!))
                logger.info {"Bid won by ${highestBidder.username} with ${bids[highestBidder]}"}
                delay(3000L)
                currentPlayer = highestBidder
                lobby.broadcast(PlayerTurn(currentPlayer!!.username))
            }
        } else {
            switchToNextPlayer()
        }
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
            hands.put(hand.key, hand.value.map { Card.fromId(it) }.toMutableList())
        }
    }

    suspend fun playCard(player: Player, cardIndex: Int) {
        val card = Card.fromId(cardIndex)
        if (player != currentPlayer) {
            throw InvalidAction("Not your turn")
        }
        if (!cardsDealing!!.hands[player]!!.contains(cardIndex)) {
            throw InvalidAction("Not your card")
        }
        if (!hands[player]!!.any { it.id == cardIndex }) {
            throw InvalidAction("Card already used")
        }
        checkActionValidity(card)
        currentLevee.add(card)
        hands[player]!!.removeIf{ it.id == cardIndex}
        lobby.broadcast(CardPlayed(cardIndex))
        if (isRoundComplete()) {
            val bestCard = tarotService.findBestCard(currentLevee)
            lobby.broadcast(PlayerTurn(bestCard.owner!!.username))
            winTurn(bestCard.owner!!)
        } else {
            switchToNextPlayer()
        }

    }

    fun winTurn(player: Player) {
        currentLevee.forEach {
            points[player]!!.add(it)
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

    private fun checkActionValidity(card: Card) {

        //Player is playing the first card
        if (currentLevee.isEmpty()) {
            if (turn == 1 && card.color == calledKing && card.isRoi()) {
                throw InvalidAction("Cannot open by called color if not playing the king")
            } else {
                return
            }
        }

        //Player is playing l'Excuse
        if (card.isExcuse()) {
            return
        }

        //First card is l'Excuse
        if (currentLevee.size == 1 && currentLevee[0].isExcuse()) {
            return
        }

        val firstCard = if (currentLevee[0].isExcuse()) currentLevee[1] else currentLevee[0]

        if (firstCard.color != card.color && hands[currentPlayer]!!.any { it.color == card.color }) {
            throw InvalidAction("Must play the same color as first card if possible")
        }

        //The player is cutting
        if (card.color == Color.ATOUT) {
            val bestAtout = currentLevee.filter { it.color == Color.ATOUT }.maxByOrNull { it.value }?.value

            //No one else played Atout
            if (bestAtout == null) {
                return

            //Player is playing a higher atout than the best already placed
            } else if (card.value > bestAtout) {
                return

            //Player is playing a lower atout than the best already played
            } else {
                if (hands[currentPlayer]!!.filter { it.color == Color.ATOUT }.any { it.value > bestAtout }) {
                    throw InvalidAction("Must play a higher atout than the best already placed")
                }
                else {
                    return
                }
            }

        //The player should no longer have any atout
        } else {
            if (hands[currentPlayer]!!.any { it.color == Color.ATOUT }) {
                throw InvalidAction("Must play atout if possible")
            } else {
                return
            }
        }
    }

}