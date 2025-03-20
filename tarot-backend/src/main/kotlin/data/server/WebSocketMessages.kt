package xclavel.data.server

import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonClassDiscriminator
import xclavel.data.tarot.Color
import xclavel.data.tarot.data.tarot.Bid

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
data class CardPlayed(val card: Int, val username: String) : WebSocketMessage()

@Serializable
@SerialName("turn_won")
data class TurnWon(val username: String) : WebSocketMessage()


@Serializable
@SerialName("dog_reveal")
data class DogReveal(val attacker: String, val cards: List<Int>) : WebSocketMessage()

@Serializable
@SerialName("dog_made")
data class DogMade(val cards: List<Int>): WebSocketMessage()

//Sent by client to declare intent to send game
//Sent by server to broadcast game start
@Serializable
@SerialName("start_game")
class StartGame() : WebSocketMessage()

//Sent by client to signal game scene is fully loaded
@Serializable
@SerialName("game_ready")
class GameReady(): WebSocketMessage()

@Serializable
@SerialName("game_over")
data class GameOver(val victory: Boolean, val score: Int, val playerScores: List<PlayerScore>): WebSocketMessage()

@Serializable
data class PlayerScore(val username: String, val score: Int)

@Serializable
@SerialName("hand_dealt")
data class HandDealt(val cards: List<Int>): WebSocketMessage()

//Bids

@Serializable
@SerialName("await_bid")
data class AwaitBid(val username: String): WebSocketMessage()

@Serializable
@SerialName("bid_made")
data class BidMade(val bid: Bid, val username: String) : WebSocketMessage()

@Serializable
@SerialName("bid_won")
data class BidWon(val username: String, val bid: Bid): WebSocketMessage()

@Serializable
@SerialName("fausse_donne")
class FausseDonne : WebSocketMessage()

//Appel

@Serializable
@SerialName("await_appel")
data class AwaitAppel(val username: String): WebSocketMessage()

@Serializable
@SerialName("appel")
data class Appel(val color: Color, val username: String): WebSocketMessage()

@Serializable
@SerialName("first_turn")
data class FirstTurn(val username: String): WebSocketMessage()