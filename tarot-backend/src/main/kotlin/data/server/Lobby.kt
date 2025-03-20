package xclavel.data.server

import kotlinx.serialization.json.Json
import io.ktor.websocket.Frame
import io.ktor.websocket.WebSocketSession
import xclavel.data.tarot.Deck
import xclavel.data.tarot.data.tarot.Bid
import xclavel.utils.logger
import java.util.concurrent.ConcurrentHashMap

class Lobby(val key: String) {
    val players = ConcurrentHashMap<String, Player>()
    val score : MutableList<Map<Player, Int>> = mutableListOf()
    var game: Game? = null
    val deck = Deck()
    var donneur: Player? = null

    val json = Json { classDiscriminator = "type" }

    fun getPlayers(): List<String> = players.values.sortedBy { it.joinTime }.map { it.username }

    fun addPlayer(player: Player) {
        players[player.username] = player
    }

    fun removePlayer(username: String) {
        players.remove(username)
    }

    suspend fun unicast(player: Player, message: WebSocketMessage) {
        player.session.sendMessage(message)
    }

    suspend fun broadcast(message: WebSocketMessage) {
        players.values.forEach { it.session.sendMessage(message) }
    }

    suspend fun WebSocketSession.sendMessage(message: WebSocketMessage) {
        val jsonMessage = json.encodeToString(WebSocketMessage.serializer(), message)
        outgoing.send(Frame.Text(jsonMessage))
    }

    fun isEmpty(): Boolean = players.isEmpty()

    suspend fun setupGame() {
        donneur = players.values.toList()[0]
        game = Game(this)
        game!!.dealCards()
        broadcast(StartGame())
    }

    suspend fun getHand(player: Player) {
        logger.info {"${player.username} is ready"}
        logger.info {"Sending hand ${game!!.cardsDealing!!.hands[player]}"}
        unicast(player, HandDealt(game!!.cardsDealing!!.hands[player]!!.toList()))
        game!!.onPlayerReady(player)
    }

    fun addGameResult(result: Map<Player, Int>) {
        score.add(result)
    }

}