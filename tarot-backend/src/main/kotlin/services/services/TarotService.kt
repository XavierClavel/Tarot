package xclavel.services.services

import org.koin.core.component.KoinComponent
import xclavel.data.tarot.Card
import xclavel.data.tarot.Color

class TarotService: KoinComponent {
    fun findBestCard(cards: List<Card>): Card {
        var bestCard = if (cards[0].isExcuse()) cards[1] else cards[0]
        val playedColor = bestCard.color
        for (i in 1 until cards.size) {
            val card = cards[i]

            if (card.isExcuse()) continue

            if (card.color == bestCard.color) {
                if (card.value > bestCard.value) {
                    bestCard = card
                    continue
                }
                continue
            }

            if (card.color == Color.ATOUT) {
                bestCard = card
                continue
            }
        }
        return bestCard
    }
}