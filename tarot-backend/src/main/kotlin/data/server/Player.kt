package xclavel.data.server

import io.ktor.websocket.WebSocketSession
import xclavel.data.tarot.Card
import java.time.Instant

data class Player(val username: String, val session: WebSocketSession) {
    val joinTime = Instant.now().toEpochMilli()
}