package xclavel.data.server

import io.ktor.websocket.WebSocketSession
import xclavel.data.tarot.Card

data class Player(val username: String, val session: WebSocketSession)