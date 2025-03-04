package xclavel

import io.ktor.server.application.*
import io.ktor.server.engine.embeddedServer
import io.ktor.server.netty.Netty
import io.ktor.server.routing.routing
import io.ktor.server.websocket.webSocket
import io.ktor.websocket.CloseReason
import io.ktor.websocket.Frame
import io.ktor.websocket.close
import io.ktor.websocket.readText
import org.koin.core.context.GlobalContext.startKoin
import xclavel.config.appModules
import xclavel.config.configureRouting
import xclavel.config.configureSockets
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
    configureSockets()
    configureRouting()

    routing {
        webSocket("/ws") { // websocketSession
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
