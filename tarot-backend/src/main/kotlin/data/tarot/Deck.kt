package xclavel.data.tarot

import xclavel.data.server.Player

class Deck {
    private val cards = mutableListOf<Card>()
    private val defausse = mutableListOf<Card>()

    init {
        for (i in 1..72) {
            cards.add(Card.fromId(i))
        }
    }

}