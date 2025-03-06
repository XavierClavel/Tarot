package xclavel.services

import org.koin.core.component.KoinComponent
import xclavel.data.server.Lobby
import xclavel.data.server.Player
import xclavel.data.server.PlayerJoined
import xclavel.data.server.PlayerLeft
import xclavel.getRandomString
import xclavel.utils.logger
import java.util.concurrent.ConcurrentHashMap

class LobbyService: KoinComponent {
    private val lobbies: ConcurrentHashMap<String, Lobby> = ConcurrentHashMap()

    fun getLobby(name: String): Lobby = lobbies[name] ?: throw Exception("Lobby '$name' not found!")

    suspend fun addPlayer(lobby: String, player: Player) {
        val lobby = getLobby(lobby)
        lobby.addPlayer(player)
        lobby.broadcast(PlayerJoined(lobby.getPlayers()))
    }

    suspend fun removePlayer(lobbyKey: String, player: Player) {
        val lobby = getLobby(lobbyKey)
        lobby.removePlayer(player.username)
        lobby.broadcast(PlayerLeft(player.username))
        if (lobby.isEmpty()) {
            deleteLobby(lobbyKey)
        }
    }

    fun createLobby(): String {
        val key = getRandomString(6)
        lobbies[key] = Lobby(key)
        logger.info { "Created Lobby '$key'" }
        return key
    }

    fun deleteLobby(lobby: String) {
        lobbies.remove(lobby)
    }


}