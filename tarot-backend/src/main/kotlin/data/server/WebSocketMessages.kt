package xclavel.data.server

import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonClassDiscriminator
import xclavel.data.tarot.data.tarot.Bid
import java.util.Dictionary

@OptIn(ExperimentalSerializationApi::class)
@Serializable
@JsonClassDiscriminator("type")
sealed class WebSocketMessage

@Serializable
@SerialName("player_joined")
data class PlayerJoined(val users: List<String>) : WebSocketMessage()

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

//Sent by client to declare intent to send game
//Sent by server to broadcast game start
@Serializable
@SerialName("start_game")
data class StartGame(val parameters: String) : WebSocketMessage()

//Sent by client to signal game scene is fully loaded
@Serializable
@SerialName("game_ready")
class GameReady(): WebSocketMessage()

@Serializable
@SerialName("game_over")
class GameOver(val victory: Boolean, val score: Int, val playerScores: Map<String, Int>): WebSocketMessage()

@Serializable
@SerialName("hand_dealt")
data class HandDealt(val cards: List<Int>): WebSocketMessage()

@Serializable
@SerialName("bid_result")
data class BidResult(val username: String, val bid: Bid): WebSocketMessage()

@Serializable
@SerialName("first_turn")
data class FirstTurn(val username: String): WebSocketMessage()