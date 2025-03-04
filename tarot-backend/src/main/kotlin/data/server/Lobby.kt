package xclavel.data.server

import io.ktor.websocket.Frame
import java.util.concurrent.ConcurrentHashMap

class Lobby(val key: String) {
    private val players = ConcurrentHashMap<String, Player>()

    fun addPlayer(player: Player) {
        players[player.username] = player
    }

    fun removePlayer(username: String) {
        players.remove(username)
    }

    suspend fun broadcastPlayerTurn(player: String) {
        broadcast("$player's turn")
    }

    suspend fun broadcastPlayerTurnWin(player: String) {
        broadcast("$player won this turn")
    }

    suspend fun broadcast(message: String) {
        players.values.forEach { it.session.send(Frame.Text(message)) }
    }

    fun isEmpty(): Boolean = players.isEmpty()
}