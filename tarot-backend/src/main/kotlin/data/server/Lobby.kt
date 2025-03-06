package xclavel.data.server

import kotlinx.serialization.json.Json
import io.ktor.websocket.Frame
import io.ktor.websocket.WebSocketSession
import java.util.concurrent.ConcurrentHashMap

class Lobby(val key: String) {
    private val players = ConcurrentHashMap<String, Player>()

    val json = Json { classDiscriminator = "type" }

    fun getPlayers(): List<String> = players.values.sortedBy { it.joinTime }.map { it.username }

    fun addPlayer(player: Player) {
        players[player.username] = player
    }

    fun removePlayer(username: String) {
        players.remove(username)
    }


    suspend fun broadcast(message: WebSocketMessage) {
        players.values.forEach { it.session.sendMessage(message) }
    }

    suspend fun WebSocketSession.sendMessage(message: WebSocketMessage) {
        val jsonMessage = json.encodeToString(WebSocketMessage.serializer(), message)
        outgoing.send(Frame.Text(jsonMessage))
    }

    fun isEmpty(): Boolean = players.isEmpty()
}