package xclavel.config

import org.koin.dsl.module
import xclavel.services.LobbyService

val appModules = module {
    single { LobbyService() }
}