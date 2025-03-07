package xclavel.data.server

import org.koin.java.KoinJavaComponent.inject
import xclavel.InvalidAction
import xclavel.data.tarot.Card
import xclavel.data.tarot.CardsDealing
import xclavel.data.tarot.Color
import xclavel.services.services.TarotService
import xclavel.utils.logger

class Game(val lobby: Lobby) {
    val tarotService by inject<TarotService>(TarotService::class.java)
    val hands = HashMap<Player, MutableList<Card>>()
    val points = HashMap<Player, MutableSet<Card>>()
    val currentLevee = mutableListOf<Card>()
    var calledKing: Color? = null
    var currentPlayer: Player? = null
    var playersOrder = mutableListOf<Player>()
    var turn = 1
    var cardsDealing: CardsDealing? = null

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
        if (isRoundComplete()) {
            val bestCard = tarotService.findBestCard(currentLevee)
            lobby.broadcast(PlayerTurn(bestCard.owner!!.username))
            winTurn(bestCard.owner!!)
        } else {
            getNextPlayer()
            lobby.broadcast(PlayerTurn(player.username))
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

    private fun getNextPlayer(): Player {
        val playerIndex = playersOrder.indexOf(currentPlayer)
        return if (playerIndex == playersOrder.size - 1) {
            playersOrder[0]
        } else {
            playersOrder[playerIndex + 1]
        }
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
        if (card.value == -1) {
            return
        }

        //First card is l'Excuse
        if (currentLevee.size == 1 && currentLevee[0].value == -1) {
            return
        }

        val firstCard = if (currentLevee[0].value == -1) currentLevee[1] else currentLevee[0]

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