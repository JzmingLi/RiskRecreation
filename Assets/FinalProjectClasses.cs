using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskRecreation
{
    public enum Player
    {
        Player1,
        Player2,
        Player3,
        Player4,
        Unoccupied
    }

    public class Country
    {
        private Player player;
        private int armies;
        private int controlValue;
        private List<Country> connectedCountries;
        private Continent continent;

        private readonly string _countryName;
        public string Name
        {
            get { return _countryName; }
        }

        public Country(string name, Continent continent) // Constructor with continent
        {
            player = Player.Unoccupied;
            _countryName = name;
            armies = 0;
            this.continent = continent;
        }
        public Country(string name, List<Country> connections, Continent continent) // Constructor with predefined connections
        {
            player = Player.Unoccupied;
            _countryName = name;
            armies = 0;
            connectedCountries = connections;
            this.continent = continent;
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
        public void AddArmies(int amount)
        {
            armies += amount * controlValue;

        }
        public void ClearArmies()
        {
            armies = 0;
        }
        public void SubtractArmies(int amount)
        {
            armies -= amount;
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

        // Players
        public void SetPlayer(Player player)
        {
            this.player = player;
        }
        public Player GetPlayer() { return player; }
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
        private Player controllingPlayer;
        private List<Country> countries;
        

        public Continent(string name, int controlValue, List<Country> countries)
        {
            _continentName = name;
            _controlValue = controlValue;
            controllingPlayer = Player.Unoccupied;
            this.countries = countries;
        }

        public void AddCountry(Country country) //Only adds if country isn't already there
        {
            if(!countries.Contains(country))
            {
                countries.Add(country);
            }
        }

        public void CheckIfControlled() //Check if all countires are controlled by the same playerw
        {
            Player player = countries[0].GetPlayer(); 
            isControlled = !(player == Player.Unoccupied);

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

        public Player GetPlayer()
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
        
        public Map(List<Country> countries, List<Continent> continents)
        {
            this.countries = countries;
            this.continents = continents;
        }
    }


}