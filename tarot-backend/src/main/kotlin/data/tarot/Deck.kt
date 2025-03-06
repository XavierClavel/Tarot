package xclavel.data.tarot

import xclavel.data.server.Player

class Deck {
    private val cards = mutableListOf<Card>()
    private val defausse = mutableListOf<Card>()


    init {
        //14 cards per color, 56 total
        for (i in 1..14) {
            cards.add(Card(i, Color.CARREAU, i))
            cards.add(Card(i, Color.COEUR, i+14))
            cards.add(Card(i, Color.TREFLE, i+28))
            cards.add(Card(i, Color.PIQUE, i+42))
        }
        //21 atouts
        for (i in 1..21) {
            cards.add(Card(i, Color.ATOUT, i+56))
        }
        cards.add(Card(-1, Color.EXCUSE, 72))
    }

}