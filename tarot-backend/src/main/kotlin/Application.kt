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
import org.koin.core.context.GlobalContext.startKoin
import org.koin.ktor.ext.inject
import xclavel.config.appModules
import xclavel.config.configureRouting
import xclavel.config.configureSockets
import xclavel.data.server.Player
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

            lobby.addPlayer(player)
            lobby.broadcast("$username joined the lobby.")

            for (frame in incoming) {
                if (frame is Frame.Text) {
                    val text = frame.readText()
                    outgoing.send(Frame.Text("YOU SAID: $text"))
                    if (text.equals("bye", ignoreCase = true)) {
                        close(CloseReason(CloseReason.Codes.NORMAL, "Client said BYE"))
                    }
                }
            }
        }
    }
}
