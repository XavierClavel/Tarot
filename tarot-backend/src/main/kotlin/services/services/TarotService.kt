package xclavel.services.services

import org.koin.core.component.KoinComponent
import xclavel.data.tarot.Card
import xclavel.data.tarot.Color

class TarotService: KoinComponent {
    fun findBestCard(cards: List<Card>): Card {
        var bestCard = cards[0]
        val playedColor = bestCard.color
        for (i in 1 until cards.size) {
            val card = cards[i]

            //Le joueur pisse
            if (card.color != playedColor && card.color != Color.ATOUT) {
                continue
            }

            //Le joueur coupe
            if (card.color == Color.ATOUT && bestCard.color != Color.ATOUT) {
                bestCard = card
                continue
            }

            //Le joueur joue la couleur
            if (card.value > bestCard.value) {
                bestCard = card
                continue
            }
        }
        return bestCard
    }
}