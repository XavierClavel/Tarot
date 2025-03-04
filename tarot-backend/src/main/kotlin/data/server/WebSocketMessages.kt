package xclavel.data.server

import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonClassDiscriminator
import xclavel.data.tarot.data.tarot.Bid

@OptIn(ExperimentalSerializationApi::class)
@Serializable
@JsonClassDiscriminator("type")
sealed class WebSocketMessage

@Serializable
@SerialName("player_joined")
data class PlayerJoined(val username: String) : WebSocketMessage()

@Serializable
@SerialName("player_left")
data class PlayerLeft(val username: String) : WebSocketMessage()

@Serializable
@SerialName("player_turn")
data class PlayerTurn(val username: String) : WebSocketMessage()

@Serializable
@SerialName("card_played")
data class CardPlayed(val card: Int) : WebSocketMessage()

@Serializable
@SerialName("turn_won")
data class TurnWon(val username: String) : WebSocketMessage()

@Serializable
@SerialName("bid_made")
data class BidMade(val bid: Bid) : WebSocketMessage()

@Serializable
@SerialName("dog_made")
data class DogMade(val cards: List<Int>): WebSocketMessage()