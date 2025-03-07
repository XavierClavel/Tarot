package xclavel.data.tarot

import xclavel.data.server.Game
import xclavel.data.server.Player

class Deck {
    private var cards = mutableListOf<Int>()
    private val defausse = mutableListOf<Int>()

    companion object {
        const val MIN_CUT_INDEX = 1 + 3
        const val MAX_CUT_INDEX = 78 - 3
    }

    init {
        cards = (1..78).toMutableList()
    }

    fun cut() {
        val cutIndex = (MIN_CUT_INDEX..MAX_CUT_INDEX).random()
        val subList1 = cards.subList(0, cutIndex)
        val subList2 = cards.subList(cutIndex, cards.size)
        cards = (subList2 + subList1) as MutableList<Int>
    }

    fun deal(players: List<Player>): CardsDealing {
        val gameMode = GameMode.getRules(players.size)
        val dogIndexes = gameMode.getDogIndexes()
        val cardsDealing = CardsDealing(HashMap(),mutableListOf())
        for (player in players) {
            cardsDealing.hands.put(player, mutableListOf())
        }
        for (i in 0 until gameMode.distributionsPerPlayer) {
            val dealIndex = i * gameMode.playersAmount
            for (j in 0 until gameMode.playersAmount) {
                val packetsDistributed = j + dealIndex
                val cardIndex = packetsDistributed * gameMode.cardsPerDistribution + cardsDealing.dog.size
                val minIndex = cardIndex
                val maxIndex = cardIndex + gameMode.cardsPerDistribution
                cardsDealing.hands[players[j]]!!.addAll(cards.subList(minIndex, maxIndex))
                if (packetsDistributed in dogIndexes) {
                    cardsDealing.dog.add(cards[cardIndex + gameMode.cardsPerDistribution])
                }
            }
        }
        return cardsDealing
    }

}