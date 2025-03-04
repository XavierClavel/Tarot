package xclavel.data.server.data.server

import io.ktor.util.collections.ConcurrentSet
import io.ktor.websocket.Frame
import io.ktor.websocket.WebSocketSession
import org.koin.java.KoinJavaComponent.inject
import xclavel.InvalidAction
import xclavel.data.server.Lobby
import xclavel.data.server.Player
import xclavel.data.tarot.Card
import xclavel.services.LobbyService
import xclavel.services.services.TarotService
import java.util.concurrent.ConcurrentHashMap
import java.util.concurrent.ConcurrentLinkedDeque

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
        currentLevee.add(card)
        if (isRoundComplete()) {
            val bestCard = tarotService.findBestCard(currentLevee)
            lobby.broadcastPlayerTurn(bestCard.owner!!.username)
        } else {
            getNextPlayer()
            lobby.broadcastPlayerTurn(player.username)
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