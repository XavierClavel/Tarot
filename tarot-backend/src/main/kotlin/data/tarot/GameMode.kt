package xclavel.data.tarot

data class GameMode(
    val playersAmount: Int,
    val cardsPerDistribution: Int,
    val cardsPerPlayer: Int,
    val tableStacksPerPlayer: Int = 0,
    val additionalCards: Int = 0,
    val dogSize: Int,
    val appel: Boolean,
    val simplePoignee: Int,
    val doublePoignee: Int,
    val triplePoignee: Int,
) {
    val distributionsPerPlayer = cardsPerPlayer / cardsPerDistribution
    val dealingsAmount = playersAmount * distributionsPerPlayer

    fun getDogIndexes(): List<Int> {
        val dogIndexes = mutableListOf<Int>()
        val possibleIndexes = (0..dealingsAmount - 3).toMutableList()
        repeat(dogSize) {
            val index = possibleIndexes.random()
            possibleIndexes.remove(index)
            dogIndexes.add(index)
        }
        return dogIndexes
    }

    companion object {
        fun getRules(playerAmount: Int) =
            when (playerAmount) {
                2 -> GameMode(
                    playersAmount = 2,
                    cardsPerDistribution = 3,
                    cardsPerPlayer = 24,
                    dogSize = 6,
                    appel = false,
                    simplePoignee = 13,
                    doublePoignee = 15,
                    triplePoignee = 18,
                    tableStacksPerPlayer = 6,
                )
                3 -> GameMode(
                    playersAmount = 3,
                    cardsPerDistribution = 4,
                    cardsPerPlayer = 24,
                    dogSize = 6,
                    appel = false,
                    simplePoignee = 13,
                    doublePoignee = 15,
                    triplePoignee = 18,
                )
                4 -> GameMode(
                    playersAmount = 4,
                    cardsPerDistribution = 3,
                    cardsPerPlayer = 18,
                    dogSize = 6,
                    appel = false,
                    simplePoignee = 10,
                    doublePoignee = 13,
                    triplePoignee = 15,
                )
                5 -> GameMode(
                    playersAmount = 5,
                    cardsPerDistribution = 3,
                    cardsPerPlayer = 15,
                    dogSize = 3,
                    appel = true,
                    simplePoignee = 8,
                    doublePoignee = 10,
                    triplePoignee = 13,
                )
                else -> throw Exception("Invalid player number")
            }
    }

}