using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiskRecreation;
using JetBrains.Annotations;
using System;
using TMPro;

public class MainGame : MonoBehaviour
{
    public GameObject text;

    void Start()
    {
        Main();
    }

    // Method to help find country for linking
    Country FindCountry(List<Country> countryList, string name)
    {
        foreach (Country country in countryList)
        {
            Debug.Log(country.Name);
            if (country.Name == name) { return country; }
        }
        Debug.Log("Country with name " + name + " doesn't exist!");
        return null;
    }

    int Main()
    {
        /* The map is already set up in Unity, but to showcase the DrawMap method that initailizes the map using code,
         * We gathered all the object's positions and sprites and fed the information to the Map class to redraw them map
         * using the method we created, and then deleted the unity made one.
         * UPDATE: Draw map may not be necessary because to clone GameObjects we have to initialize them anyway
         * The other way we could do it is assigning prefabs in serialized fields but thats a Unity only thing so I don't think its worth it
         */

        //Getting Continents by getting all with tag
        GameObject[] continentGameObjects = GameObject.FindGameObjectsWithTag("Continent");

        //Getting Countries by getting all children of each continent
        List<List<GameObject>> countryGameObjects = new List<List<GameObject>>();
        foreach (GameObject continent in continentGameObjects)
        {
            List<GameObject> continentCountries = new List<GameObject>();
            foreach (Transform child in continent.transform)
            {
                continentCountries.Add(child.gameObject);
            }
            countryGameObjects.Add(continentCountries);
        }

        // These lists will be passed onto the map class later
        List<Continent> continentList = new List<Continent>();
        List<Country> countryList = new List<Country>();

        foreach (GameObject continentGameObject in continentGameObjects)
        {
            // Get the list of country gameobjects for the continent
            List<GameObject> countries = countryGameObjects[Array.IndexOf(continentGameObjects, continentGameObject)];
            // Here the original object is cloned and the clone is set as the actual continent object
            // Also the control value is just set to the amount of countries in the continent
            Continent continent = new Continent(continentGameObject.name, countries.Count, Instantiate(continentGameObject, continentGameObject.transform.position, Quaternion.identity));
            // Make the actual country objects (position is required in the constructor but after realizing draw map is redundant it may be useless)
            // Can't hurt to have it though, also add the countries to the country list
            foreach(GameObject country in countries)
            {
                Country newCountry = new Country(country.name, continent, new Vector2(country.transform.position.x, country.transform.position.y), Instantiate(country));
                continent.AddCountry(newCountry);
                countryList.Add(newCountry);
            }
            foreach (Transform child in continent.Sprite.transform)
            {
                // Add nametags for clarity
                GameObject nametag = Instantiate(text, child.position + new Vector3(0,-0.5f,0), child.rotation);
                nametag.GetComponent<TextMeshPro>().text = child.gameObject.name;
                // Destroy the children in the new instance (Noticed this bug after playtesting)
                Destroy(child.gameObject);
            }
            // It shouldn't matter because it passes a reference to the continent to the list, but just in case now we add the continent with all of its countries
            continentList.Add(continent);
        }

        // Delete the original editor made objects
        foreach (GameObject continentGameObject in continentGameObjects)
        {
            Destroy(continentGameObject);
        }

        // Manual Connection Assignment

        foreach(Country country in countryList)
        {
            string countryName = country.Name;
            switch (countryName)
            {
                // Aerthos
                case "Solara":
                    country.MakeConnection(FindCountry(countryList, "Verdantia"));
                    country.MakeConnection(FindCountry(countryList, "Lumina"));
                    break;
                case "Verdanita":
                    country.MakeConnection(FindCountry(countryList, "Lumina"));
                    country.MakeConnection(FindCountry(countryList, "Terranova"));
                    break;
                case "Lumina":
                    country.MakeConnection(FindCountry(countryList, "Celestia"));
                    country.MakeConnection(FindCountry(countryList, "Terranova"));
                    break;
                case "Celestia":
                    country.MakeConnection(FindCountry(countryList, "Terranova"));
                    country.MakeConnection(FindCountry(countryList, "Sylvaria"));
                    break;
                case "Terranova":
                    country.MakeConnection(FindCountry(countryList, "Sylvaria"));
                    break;
                case "Sylvaria":
                    country.MakeConnection(FindCountry(countryList, "Mythos"));
                    country.MakeConnection(FindCountry(countryList, "Helios"));
                    break;
                // Novaris
                case "Mythos":
                    country.MakeConnection(FindCountry(countryList, "Meridian"));
                    country.MakeConnection(FindCountry(countryList, "Arcadia"));
                    country.MakeConnection(FindCountry(countryList, "Aurelia"));
                    country.MakeConnection(FindCountry(countryList, "Crystallia"));
                    break;
                case "Meridian":
                    country.MakeConnection(FindCountry(countryList, "Arcadia"));
                    country.MakeConnection(FindCountry(countryList, "Valoria"));
                    break;
                case "Arcadia":
                    country.MakeConnection(FindCountry(countryList, "Valoria"));
                    country.MakeConnection(FindCountry(countryList, "Solstice"));
                    country.MakeConnection(FindCountry(countryList, "Aurelia"));
                    break;
                case "Aurelia":
                    country.MakeConnection(FindCountry(countryList, "Solstice"));
                    break;
                case "Solstice":
                    country.MakeConnection(FindCountry(countryList, "Valoria"));
                    break;
                // Zephyria
                case "Crystallia":
                    country.MakeConnection(FindCountry(countryList, "Helios"));
                    country.MakeConnection(FindCountry(countryList, "Aquatia"));
                    country.MakeConnection(FindCountry(countryList, "Chronos"));
                    break;
                case "Chronos":
                    country.MakeConnection(FindCountry(countryList, "Aetheria"));
                    break;
                case "Aetheria":
                    country.MakeConnection(FindCountry(countryList, "Emberia"));
                    break;
                case "Emberia":
                    country.MakeConnection(FindCountry(countryList, "Nocturnia"));
                    country.MakeConnection(FindCountry(countryList, "Aquatia"));
                    break;
                case "Aquatia":
                    country.MakeConnection(FindCountry(countryList, "Nocturnia"));
                    break;
                // Eldoria
                case "Helios":
                    country.MakeConnection(FindCountry(countryList, "Seraphia"));
                    country.MakeConnection(FindCountry(countryList, "Umbra"));
                    break;
                case "Umbra":
                    country.MakeConnection(FindCountry(countryList, "Tempestia"));
                    country.MakeConnection(FindCountry(countryList, "Seraphia"));
                    break;
                case "Tempestia":
                    country.MakeConnection(FindCountry(countryList, "Solaris"));
                    country.MakeConnection(FindCountry(countryList, "Vortexia"));
                    country.MakeConnection(FindCountry(countryList, "Seraphia"));
                    break;
                case "Solaris":
                    country.MakeConnection(FindCountry(countryList, "Elysium"));
                    country.MakeConnection(FindCountry(countryList, "Vortexia"));
                    country.MakeConnection(FindCountry(countryList, "Nexus"));
                    break;
                case "Elysium":
                    country.MakeConnection(FindCountry(countryList, "Nexus"));
                    break;
                case "Vortexia":
                    country.MakeConnection(FindCountry(countryList, "Nexus"));
                    country.MakeConnection(FindCountry(countryList, "Seraphia"));
                    break;
                default:
                    Debug.Log("No properly named countries!");
                    break;
            }
        }

        // Add the Map
        Map gameMap = new Map(countryList, continentList);

        // Choose player colours
        gameMap.ChoosePlayerColors(Color.red, Color.blue, Color.yellow, Color.green, Color.gray);

        gameMap.UpdateCountryLabels();
        gameMap.ShowContinentPlayerControl();

        return 0;
    }
}
