using System;

/// <summary>
/// This class represents a card.
/// </summary>
public class CardInfo
{
    /// <summary>
    /// Card rarities
    /// </summary>
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        LEGENDARY
    }
    
    /// <summary>
    /// Id of card in database.
    /// </summary>
    private int cardId;
    
    /// <summary>
    /// Name of the card.
    /// </summary>
    private string name;
    
    /// <summary>
    /// Rarity of the card.
    /// </summary>
    private Rarity rarity;
    
    /// <summary>
    /// URL of the picture of the card.
    /// </summary>
    private string picURL;
    
    /// <summary>
    /// Val1 of the card.
    /// </summary>
    private int val1;
    
    /// <summary>
    /// Val2 of the card.
    /// </summary>
    private int val2;
    
    /// <summary>
    /// Val3 of the card.
    /// </summary>
    private int val3;
    
    
    public int CardId => cardId;
    
    public string Name => name;

    public Rarity CardRarity => rarity;

    public string PicUrl => picURL;

    public int Val1 => val1;

    public int Val2 => val2;

    public int Val3 => val3;
    
    /// <summary>
    /// Creates a card with information from the database.
    /// </summary>
    /// <param name="cardId">Id of card</param>
    /// <param name="name">Name of card</param>
    /// <param name="rarityNumber">Idx of rarity</param>
    /// <param name="picURL">URL of card picture</param>
    /// <param name="val1">Value 1 of card</param>
    /// <param name="val2">Value 2 of card</param>
    /// <param name="val3">Value 3 of card</param>
    public CardInfo(int cardId, string name, int rarityNumber, string picURL, int val1, int val2, int val3)
    {
        this.cardId = cardId;
        this.name = name;
        this.picURL = picURL;
        this.val1 = val1;
        this.val2 = val2;
        this.val3 = val3;

        this.rarity = (Rarity) rarityNumber;
    }
    
    public int valForIdx(int valIdx)
    {
        switch (valIdx)
        {
            case 1:
                return val1;
            case 2:
                return val2;
            case 3:
                return val3;
            default:
                throw new Exception("VALUE OUT OF RANGE");
        }
    }
}
