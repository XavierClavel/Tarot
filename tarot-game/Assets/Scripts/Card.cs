public class Card
{
    public readonly int index;
    public readonly int value;
    public readonly TarotColor color;
    
    public Card(int index)
    {
        this.index = index;
        switch (index)
        {
            case <= 14:
                this.color = TarotColor.CARREAU;
                this.value = index;
                break;
            
            case <= 28:
                this.color = TarotColor.TREFLE;
                this.value = index - 14;
                break;
            
            case <= 42:
                this.color = TarotColor.COEUR;
                this.value = index - 28;
                break;
            
            case <= 56:
                this.color = TarotColor.PIQUE;
                this.value = index - 42;
                break;
            
            case <= 77:
                this.color = TarotColor.ATOUT;
                this.value = index - 56;
                break;
            
            case 78:
                this.color = TarotColor.EXCUSE;
                this.value = 78;
                break;
        }
    }
}
