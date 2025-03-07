package xclavel

import io.ktor.server.application.*
import io.ktor.server.engine.embeddedServer
import io.ktor.server.netty.Netty
import io.ktor.server.response.respond
import io.ktor.server.routing.post
import io.ktor.server.routing.routing
import io.ktor.server.websocket.webSocket
import io.ktor.websocket.CloseReason
import io.ktor.websocket.Frame
import io.ktor.websocket.close
import io.ktor.websocket.readText
import kotlinx.coroutines.channels.ClosedReceiveChannelException
import kotlinx.serialization.json.Json
import org.koin.core.context.GlobalContext.startKoin
import org.koin.ktor.ext.inject
import xclavel.config.appModules
import xclavel.config.configureRouting
import xclavel.config.configureSockets
import xclavel.data.server.BidMade
import xclavel.data.server.CardPlayed
import xclavel.data.server.DogMade
import xclavel.data.server.GameReady
import xclavel.data.server.HandDealt
import xclavel.data.server.Player
import xclavel.data.server.PlayerJoined
import xclavel.data.server.PlayerLeft
import xclavel.data.server.PlayerTurn
import xclavel.data.server.StartGame
import xclavel.data.server.TurnWon
import xclavel.data.server.WebSocketMessage
import xclavel.services.LobbyService
import xclavel.utils.logger

fun main(args: Array<String>) {
    startKoin {
        modules(
            appModules
        )}


    logger.info { " Server started." }
    embeddedServer(Netty, port = 3535, host = "0.0.0.0", module = Application::module)
        .start(wait = true)
    logger.info { " Server closed." }
}

fun Application.module() {
    val lobbyService by inject<LobbyService>()
    val json = Json { classDiscriminator = "type" }

    configureSockets()
    configureRouting()

    routing {
        post("/lobby") {
            call.respond(lobbyService.createLobby())
        }

        webSocket("/lobby/{lobbyKey}/{username}") {
            val lobbyKey = call.parameters["lobbyKey"]?.trim() ?: return@webSocket close(CloseReason(CloseReason.Codes.VIOLATED_POLICY, "Invalid lobby"))
            val username = call.parameters["username"]?.trim() ?: return@webSocket close(CloseReason(CloseReason.Codes.VIOLATED_POLICY, "Invalid username"))

            logger.info {"'$username' is logging in to lobby '$lobbyKey'"}

            val lobby = lobbyService.getLobby(lobbyKey)
            val player = Player(username, this)
            lobbyService.addPlayer(lobbyKey, player)

            try {
                for (frame in incoming) {
                    if (frame is Frame.Text) {
                        val text = frame.readText()
                        try {
                            val message = json.decodeFromString<WebSocketMessage>(text)

                            when (message) {
                                is PlayerJoined -> TODO()
                                is PlayerLeft -> lobby.broadcast(message)
                                is BidMade -> TODO()
                                is CardPlayed -> TODO()
                                is DogMade -> TODO()
                                is PlayerTurn -> TODO()
                                is TurnWon -> TODO()
                                is StartGame -> lobby.setupGame()
                                is GameReady -> lobby.getHand(player)
                                is HandDealt -> TODO()
                            }
                        } catch (e: Exception) {
                            println("Error decoding WebSocket message: $e")
                        }
                    }
                }
            } finally {
                val reason = closeReason.await()
                lobbyService.removePlayer(lobbyKey,player)
                logger.info {"websocket closed by user $username for reason ${reason?.message}"}
            }
        }
    }
}

//TODO: petit sec
//TODO: afficher atouts dans le chien
//TODO: chelem
//TODO: poignées
//TODO: petit au bout
//TODO: enchères
//TODO: 2 players mode