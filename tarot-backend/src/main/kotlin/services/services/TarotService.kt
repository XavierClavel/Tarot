package xclavel.services.services

import org.koin.core.component.KoinComponent
import xclavel.InvalidAction
import xclavel.data.tarot.Card
import xclavel.data.tarot.Color
import xclavel.data.tarot.data.tarot.Bid
import kotlin.collections.any
import kotlin.collections.filter
import kotlin.math.ceil
import kotlin.math.floor

class TarotService: KoinComponent {
    fun findBestCard(cards: List<Card>): Card {
        var bestCard = if (cards[0].isExcuse()) cards[1] else cards[0]
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

    fun checkActionValidity(card: Card, currentLevee: List<Card>, hand: List<Card>, turn: Int, calledKing: Color?) {

        //Player is playing the first card
        if (currentLevee.isEmpty()) {
            if (turn == 1 && card.color == calledKing && !card.isRoi()) {
                throw InvalidAction("Cannot open by called color if not playing the king")
            } else {
                return
            }
        }

        //Player is playing l'Excuse
        if (card.isExcuse()) {
            return
        }

        //First card is l'Excuse
        if (currentLevee.size == 1 && currentLevee[0].isExcuse()) {
            return
        }

        val firstCard = if (currentLevee[0].isExcuse()) currentLevee[1] else currentLevee[0]

        if (card.isRegularCard() && card.color == firstCard.color) {
            return
        }

        if (card.color != firstCard.color && hand.any { it.color == firstCard.color }) {
            throw InvalidAction("Must play the same color as first card if possible")
        }

        //The player is cutting
        if (card.color == Color.ATOUT) {
            val bestAtout = currentLevee.filter { it.color == Color.ATOUT }.maxByOrNull { it.value }?.value

            //No one else played Atout
            if (bestAtout == null) {
                return

                //Player is playing a higher atout than the best already placed
            } else if (card.value > bestAtout) {
                return

                //Player is playing a lower atout than the best already played
            } else {
                if (hand.filter { it.color == Color.ATOUT }.any { it.value > bestAtout }) {
                    throw InvalidAction("Must play a higher atout than the best already placed")
                }
                else {
                    return
                }
            }

            //The player should no longer have any atout
        } else {
            if (hand.any { it.color == Color.ATOUT }) {
                throw InvalidAction("Must play atout if possible")
            } else {
                return
            }
        }
    }

    fun countPoints(cards: List<Card>, threshold: Int): Int {
        val points = cards.sumOf { it.countPoint().toDouble() }
        return if (points > threshold) ceil(points).toInt()
            else floor(points).toInt()
    }

    fun calculateScore(points: Int, winThreshold: Int, contract: Bid, playersAmount: Int): Int {
        val delta = points - winThreshold
        return (25 + delta) * contract.multiplier
    }


    fun getWinThreshold(oudlersCount: Int): Int =
        when (oudlersCount) {
            0 -> 56
            1 -> 51
            2 -> 41
            3 -> 36
            else -> throw Exception("Impossible amount of oudlers")
        }

    fun getPlayerAmountMultiplier(playerAmount: Int): Int =
        when(playerAmount) {
            2 -> 1
            3 -> 2
            4 -> 3
            5 -> 3
            else -> throw Exception("Impossible amount of player")
        }



    fun countOudlers(cards: List<Card>): Int =
        cards.count { it.isOudler() }

}