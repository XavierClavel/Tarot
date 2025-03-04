package xclavel.data.server

import io.ktor.websocket.WebSocketSession

data class Player(val username: String, val session: WebSocketSession)
