package xclavel.data.server

import org.koin.java.KoinJavaComponent.inject
import xclavel.InvalidAction
import xclavel.data.tarot.Card
import xclavel.services.services.TarotService
import java.util.concurrent.ConcurrentHashMap

class Game(val lobby: Lobby) {
    val tarotService by inject<TarotService>(TarotService::class.java)
    val cards = ConcurrentHashMap<Int, Card>()
    val hands = HashMap<Player, MutableSet<Card>>()
    val points = HashMap<Player, MutableSet<Card>>()
    val currentLevee = mutableListOf<Card>()
    var currentPlayer: Player? = null
    var playersOrder = mutableListOf<Player>()

    suspend fun playCard(player: Player, card: Card) {
        if (player != currentPlayer) {
            throw InvalidAction("Not your turn")
        }
        if (card.owner != currentPlayer) {
            throw InvalidAction("Not your card")
        }
        currentLevee.add(card)
        if (isRoundComplete()) {
            val bestCard = tarotService.findBestCard(currentLevee)
            lobby.broadcast(PlayerTurn(bestCard.owner!!.username))
        } else {
            getNextPlayer()
            lobby.broadcast(PlayerTurn(player.username))
        }

    }

    fun winLevee(player: Player) {
        currentLevee.forEach {
            points[player]!!.add(it)
        }
        currentLevee.clear()
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

}