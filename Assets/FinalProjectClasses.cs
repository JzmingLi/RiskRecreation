using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace RiskRecreation
{
    public enum PlayerSlot
    {
        Player1,
        Player2,
        Player3,
        Player4,
        Unoccupied
    }

    public enum Card
    {
        Infantry,
        Cavalry,
        Artillery
    }

    public class Country
    {
        private PlayerSlot player;
        private int armies;
        private List<Country> connectedCountries;
        private Continent continent;

        private readonly string _countryName;
        public string Name
        {
            get { return _countryName; }
        }

        private readonly Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
        }

        private readonly GameObject _sprite;
        public GameObject Sprite
        {
            get { return _sprite; }
        }

        public Country(string name, Continent continent, Vector2 position, GameObject sprite) // Constructor with continent
        {
            player = PlayerSlot.Unoccupied;
            _countryName = name;
            armies = 0;
            this.continent = continent;
            _position = position;
            _sprite = sprite;
        }
        public Country(string name, List<Country> connections, Continent continent, Vector2 position, GameObject sprite) // Constructor with predefined connections
        {
            player = PlayerSlot.Unoccupied;
            _countryName = name;
            armies = 0;
            connectedCountries = connections;
            this.continent = continent;
            _position = position;
            _sprite = sprite;
        }

        public Country()
        {
            connectedCountries = new List<Country>();
        }

        // Managing Connections
        public void AddConnection(Country connection)
        {
            connectedCountries.Add(connection);
        }
        public void MakeConnection(Country connection) // Ensures the country its making a connection with also adds them as a connection
        {
            if (!CheckConnection(connection))
            {
                AddConnection(connection);
            }
            if (!connection.CheckConnection(this))
            {
                connection.AddConnection(this);
            }
        }

        // Will be useful for when player is choosing a country to move/invade to
        public List<Country> GetConnections()
        {
            return connectedCountries;
        }
        public bool CheckConnection(Country connection)
        {
            return connectedCountries.Contains(connection);
        }

        // Armies
        public void AddArmies(int amount) // Add armies to country
        {
            armies += amount;

        }
        public void ClearArmies()
        {
            armies = 0;
        }
        public void SubtractArmies(int amount)
        {
            armies -= amount;
            if (armies < 0) Debug.Log("Negative Armies");
        }
        public void SetArmies(int amount)
        {
            armies = amount;
        }
        public bool TransferArmies(int amount, Country targetCountry) // For fortification phase, returns false if unable to preform action
        {
            if (amount > armies)
            {
                return false;
            }
            else
            {
                armies -= amount;
                targetCountry.AddArmies(amount);
                return true;
            }
        }
        public int Armies()
        {
            return armies; 
        }

        // Players
        public void SetPlayer(PlayerSlot player)
        {
            this.player = player;
        }
        public PlayerSlot GetPlayer() { return player; }
    }

    public class Continent
    {
        private readonly string _continentName;
        public string Name
        {
            get { return _continentName; }
        }
        private readonly int _controlValue;
        public int ControlValue
        {
            get { return _controlValue; }
        }

        private bool isControlled;
        private PlayerSlot controllingPlayer;
        private List<Country> countries;
        private readonly GameObject _sprite;
        public GameObject Sprite
        {
            get { return _sprite; }
        }

        public Continent(string name, int controlValue, List<Country> countries, GameObject sprite)
        {
            _continentName = name;
            _controlValue = controlValue;
            controllingPlayer = PlayerSlot.Unoccupied;
            this.countries = countries;
            _sprite = sprite;
        }

        public void AddCountry(Country country) //Only adds if country isn't already there
        {
            if(!countries.Contains(country))
            {
                countries.Add(country);
            }
        }

        public void CheckIfControlled() //Check if all countires are controlled by the same player
        {
            PlayerSlot player = countries[0].GetPlayer(); 
            isControlled = !(player == PlayerSlot.Unoccupied);

            foreach (Country country in countries)
            {
                if (country.GetPlayer() != player)
                {
                    isControlled = false;
                }
            }

            if (isControlled)
            {
                controllingPlayer = countries[0].GetPlayer();
            }
        }

        public PlayerSlot GetPlayer()
        {
            return controllingPlayer;
        }

        public bool GetIsControlled()
        {
            return isControlled;
        }
    }

    public class Map
    {
        List<Country> countries;
        List<Continent> continents;

        public List<GameObject> continentSprites;
        public List<GameObject> countrySprites;
        
        public Map(List<Country> countries, List<Continent> continents)
        {
            this.countries = countries;
            this.continents = continents;
        }

        private Dictionary<PlayerSlot, Color> PlayerColors = new Dictionary<PlayerSlot, Color>(); // Determine player colours

        public void ChoosePlayerColors(Color player1, Color player2, Color player3, Color player4, Color unoccupied) // Called by main to assign player colors
        {
            PlayerColors.Add(PlayerSlot.Player1, player1);
            PlayerColors.Add(PlayerSlot.Player2, player2);
            PlayerColors.Add(PlayerSlot.Player3, player3);
            PlayerColors.Add(PlayerSlot.Player4, player4);
            PlayerColors.Add(PlayerSlot.Unoccupied, unoccupied);
        }

        // Instantiate all visuals and game objects
        public void DrawMap()
        {
            foreach(Continent continent in continents)
            {
                continentSprites.Add(Object.Instantiate(continent.Sprite));
            }

            foreach(Country country in countries)
            {
                Vector3 positionIn3D = new Vector3(country.Position.x, country.Position.y, 0);
                countrySprites.Add(Object.Instantiate(country.Sprite, positionIn3D, Quaternion.identity));
            }
        }

        // Update Army Counters and Colors
        public void UpdateCountryLabels()
        {
            foreach(Country country in countries)
            {
                // Counter
                GameObject countrySprite = countrySprites[countries.IndexOf(country)];
                countrySprite.GetComponentInChildren<TextMeshPro>().text = country.Armies().ToString();

                //Color
                Color playerColor = PlayerColors[country.GetPlayer()];
                countrySprite.GetComponent<SpriteRenderer>().color = playerColor;
            }
        }

        // Change color of continent sprite if all countries within have the same owner
        public void ShowContinentPlayerControl()
        {
            foreach (Continent continent in continents)
            {
                continent.CheckIfControlled();
                if (continent.GetIsControlled())
                {
                    Color playerColor = PlayerColors[continent.GetPlayer()];
                    GameObject continentSprite = continentSprites[continents.IndexOf(continent)];
                    continentSprite.GetComponent<SpriteRenderer>().color = playerColor;
                }
            }
        }
    }

    public class Player
    {
        private List<Country> countries;
        private int armies; // Should be 0 at the end of a turn
        private int exchangeCounter; //Should be 0 at the start of every turn
        private int InfantryCards;
        private int ArtilleryCards;
        private int CavalryCards;

        public Player(List<Country> countries)
        {
            this.countries = countries;
            armies = 0;
            exchangeCounter = 0;
            InfantryCards = 0;
            ArtilleryCards = 0;
            CavalryCards = 0;
        }

        public void AddCountry(Country country)
        {
            countries.Add(country); 
        }

        public void RemoveCountry(Country country)
        {
            countries.Remove(country);
        }

        // Armies, not all functions necessary but do exactly as they are supposed to
        // In main, the armies will be added and then the player will subtract armies to reinforce countries
        public bool CheckArmiesDepleted()
        {
            return armies == 0;
        }

        public void AddArmies(int amount)
        {
            armies += amount;

        }

        public void SetArmies(int amount)
        {
            armies = amount;
        }

        public void SubtractArmies(int amount)
        {
            armies -= amount;
            if (armies < 0) Debug.Log("Negative Armies");
        }

        //Function to tally total armies in case
        public int GetTotalArmies()
        {
            int totalArmies = 0;
            foreach(Country country in countries)
            {
                totalArmies += country.Armies();
            }
            return totalArmies;
        }

        //Use cards to get armies
        public void UseCards(Card card)
        {
            switch (card)
            {
                case Card.Infantry:
                    InfantryCards -= 3;
                    break;
                case Card.Cavalry:
                    CavalryCards -= 3;
                    break;
                case Card.Artillery:
                    ArtilleryCards -= 3;
                    break;
            }

            armies += 5 * exchangeCounter; // 5 extra for each subsequent exchange
        }

        public List<Card> Cards()
        {
            List<Card> cards = new List<Card>();
            for (int i = 0; i < InfantryCards; i++) cards.Add(Card.Infantry);
            for (int i = 0; i < ArtilleryCards; i++) cards.Add(Card.Artillery);
            for (int i = 0; i < CavalryCards; i++) cards.Add(Card.Cavalry);

            return cards;
        }

        public void AddCard(Card card)
        {
            switch (card)
            {
                case Card.Infantry: 
                    InfantryCards++; 
                    break;
                case Card.Cavalry: 
                    CavalryCards++; 
                    break;
                case Card.Artillery:
                    ArtilleryCards++; 
                    break;
            }
        }

        public void ResetExchangeCounter()
        {
            exchangeCounter = 0;
        }
    }
}