package xclavel.data.tarot.data.tarot

enum class Bid(val multiplier: Int) {
    PASSE(0),
    PETITE(1),
    GARDE(2),
    GARDE_SANS(4),
    GARDE_CONTRE(6);
}
