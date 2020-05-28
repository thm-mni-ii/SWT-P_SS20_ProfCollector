using UnityEngine;
using System;
using System.Collections.Generic;

public class Card
{
	private String name;
	private String description;
	private int rarity;
	private Texture2D image;

	/**
	 * The Representation of ingame collectable cards. Each card thats available 
	 * has its own name, description, level of rarity and Design.
	 * Cards can be used to play the Profcollector-Quartet game as well as
	 * they can be traded or gathered. 
	 */
	public Card(String name, String description, int rarity, Texture2D image)
	{
		this.name = name;
		this.description = description;
		this.rarity = rarity;
		this.image = image;
	}

	//Getter
	public String getName()
    {
		return this.name;
    }

	public String getDiscription()
    {
		return this.description;
    }

	public int getRarity()
    {
		return this.rarity;
    }

	public Texture2D getImage()
    {
		return this.image;
    }

	//Setter
	public setName(String name)
	{
		this.name = name;
	}

	public setDiscription(String description)
	{
		this.description = description;
	}

	public setRarity(int rarity)
	{
		this.rarity = rarity;
	}

	public setImage(Texture2D image)
	{
		this.image = image;
	}
}