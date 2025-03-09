package xclavel.config

import org.koin.dsl.module
import xclavel.services.LobbyService
import xclavel.services.services.TarotService

val appModules = module {
    single { LobbyService() }
    single { TarotService() }
}