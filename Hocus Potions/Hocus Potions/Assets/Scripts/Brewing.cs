using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brewing : MonoBehaviour {
    public Sprite image;

    void Start() {
        //This is just for testing purposes  
        Ingredient lavender = new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.healing, Ingredient.Attributes.chicken }, "lavender", image);
        Ingredient catnip = new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.transformation, Ingredient.Attributes.cat }, "catnip", image);
        Ingredient nightshade = new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.sleep, Ingredient.Attributes.healing }, "nightshade", image);
        Ingredient mugwort = new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.magicPP, Ingredient.Attributes.transformation }, "mugwort", image);
        Ingredient lambsgrass = new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisible, Ingredient.Attributes.healing, Ingredient.Attributes.sheep }, "lambsgrass", image);
        Ingredient poppy = new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisible, Ingredient.Attributes.poison, Ingredient.Attributes.sleep }, "poppy", image);

        Brew(lambsgrass, lambsgrass, poppy);
    }

class attList {
        public int count;
        public Ingredient.Attributes attribute;

        public attList(int c, Ingredient.Attributes a) {
            count = c;
            attribute = a;
        }
    }

    class modList {
        public int count;
        public Ingredient.Modifiers attribute;

        public modList(int c, Ingredient.Modifiers a) {
            count = c;
            attribute = a;
        }
    }

   

   
    void Brew(Ingredient first, Ingredient second, Ingredient third) {
        List<attList> attributes = new List<attList>();
        List<modList> modifiers = new List<modList>();
        Ingredient.Attributes[] attArray = new Ingredient.Attributes[] { first.attributeList[0], first.attributeList[1], first.attributeList[2],
                                                                        second.attributeList[0], second.attributeList[1], second.attributeList[2],
                                                                        third.attributeList[0], third.attributeList[1], third.attributeList[2]};

        Ingredient.Attributes primary;
        Ingredient.Attributes? secondary = null;
        Ingredient.Modifiers? mod = null;
        int duration = 0;
        //Sprite image;
        string name = "";

        bool add;
        for(int i = 0; i < 9; i++) {
            add = true;
            if (System.Enum.IsDefined(typeof(Ingredient.Modifiers), attArray[i].ToString())) {
                foreach (modList m in modifiers) {
                    if (m.attribute == (Ingredient.Modifiers)System.Enum.Parse(typeof(Ingredient.Modifiers), attArray[i].ToString())) {
                        m.count++;
                        add = false;
                        break;
                    }
                }
                if (add) {
                    modifiers.Add(new modList(1, (Ingredient.Modifiers)System.Enum.Parse(typeof(Ingredient.Modifiers), attArray[i].ToString())));
                }
            } else {
                foreach (attList a in attributes) {
                    if (a.attribute == attArray[i]) {
                        a.count++;
                        add = false;
                        break;
                    }
                }
                if (add) {
                    attributes.Add(new attList(1, attArray[i]));
                }
            }
        }

        List<attList> sortedAtt = attributes.OrderByDescending(attList => attList.count).ToList();
        List<modList> sortedMod = modifiers.OrderByDescending(modList => modList.count).ToList();

        if (sortedAtt.Count() == 0) {
            Debug.Log("Error, no attributes");
        } else {
            if (sortedAtt[0].count >= 3) {
                //accounts for ties in primary attribute
                if (sortedAtt.Count() > 1 && sortedAtt[0].count == sortedAtt[1].count) {
                    if (sortedAtt.Count() > 2 && sortedAtt[1].count == sortedAtt[2].count) {  //three-way tie
                        int flip1 = Random.Range(0, 3);
                        int flip2 = Random.Range(0, 2);
                        switch (flip1) {
                            case 0:
                                primary = sortedAtt[0].attribute;
                                if (flip2 == 0) {
                                    secondary = sortedAtt[1].attribute;
                                } else {
                                    secondary = sortedAtt[2].attribute;
                                }
                                break;
                            case 1:
                                primary = sortedAtt[1].attribute;
                                if (flip2 == 0) {
                                    secondary = sortedAtt[0].attribute;
                                } else {
                                    secondary = sortedAtt[2].attribute;
                                }
                                break;
                            case 2:
                                primary = sortedAtt[2].attribute;
                                if (flip2 == 0) {
                                    secondary = sortedAtt[0].attribute;
                                } else {
                                    secondary = sortedAtt[1].attribute;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    int flip = Random.Range(0, 2);      //two-way tie
                    if (flip == 0) {
                        primary = sortedAtt[0].attribute;
                        secondary = sortedAtt[1].attribute;
                    } else {
                        primary = sortedAtt[1].attribute;
                        secondary = sortedAtt[0].attribute;
                    }
                } else {
                    primary = sortedAtt[0].attribute;          //no tie


                    if (sortedAtt.Count() == 1) {
                        secondary = null;
                    } else {
                        if (sortedAtt[1].count >= 2) {
                            if (sortedAtt.Count() > 2 && sortedAtt[1].count == sortedAtt[2].count) {
                                if (sortedAtt.Count() > 3 && sortedAtt[2].count == sortedAtt[3].count) {    //three-way tie
                                    int flip = Random.Range(0, 3);
                                    switch (flip) {
                                        case 0:
                                            secondary = sortedAtt[1].attribute;
                                            break;
                                        case 1:
                                            secondary = sortedAtt[2].attribute;
                                            break;
                                        case 2:
                                            secondary = sortedAtt[3].attribute;
                                            break;
                                        default:
                                            Debug.Log("how is this possible");
                                            break;
                                    }
                                } else {
                                    int flip = Random.Range(0, 2);          //two-way tie
                                    if (flip == 0) {
                                        secondary = sortedAtt[1].attribute;
                                    } else {
                                        secondary = sortedAtt[2].attribute;
                                    }
                                }
                            } else {
                                secondary = sortedAtt[1].attribute;         //no tie
                            }
                            //Secondary is null if no other attribute appears 2 or more times
                        } else {
                            secondary = null;
                        }
                    }
                }
                //Modifiers - *should be functional but this will need some major tweaking in the long run*
                //Any combinations of 2 mugwort + 1 poppy or 1 nightshade end up as transformation secondaries with no animal modifier 
                if (sortedMod.Count < 1) {
                    mod = null;
                } else {
                    if (primary == Ingredient.Attributes.transformation || secondary == Ingredient.Attributes.transformation) {
                        foreach (modList m in sortedMod) {
                            if (m.attribute == Ingredient.Modifiers.cat || m.attribute == Ingredient.Modifiers.sheep || m.attribute == Ingredient.Modifiers.chicken) {
                                mod = m.attribute;
                                break;
                            }
                        }
                    } else {
                        foreach (modList m in sortedMod) {
                            if (m.attribute == Ingredient.Modifiers.magicMM || m.attribute == Ingredient.Modifiers.magicPP) {
                                mod = m.attribute;
                                break;
                            }
                        }
                    }
                }


                //Potion name generation *Rough Draft just for functionality; names not final*
                name = primary.ToString() + " potion of " + secondary.ToString();
                switch (mod) {
                    case Ingredient.Modifiers.magicPP:
                        name = "Super " + name;
                        break;
                    case Ingredient.Modifiers.magicMM:
                        name = "Weak " + name;
                        break;
                    case Ingredient.Modifiers.cat:
                        name = "Cat " + name;
                        break;
                    case Ingredient.Modifiers.sheep:
                        name = "Sheep " + name;
                        break;
                    case Ingredient.Modifiers.chicken:
                        name = "Chicken " + name;
                        break;
                    default:
                        break;
                }

                //Duration calculation *Default values atm, needs to be modified later*
                switch (primary) {
                    case Ingredient.Attributes.healing:
                        duration = 1;
                        break;
                    case Ingredient.Attributes.sleep:
                        duration = 1;
                        break;
                    case Ingredient.Attributes.invisible:
                        duration = 1;
                        break;
                    case Ingredient.Attributes.poison:
                        duration = 1;
                        break;
                    case Ingredient.Attributes.transformation:
                        duration = 1;
                        break;
                    default:
                        duration = 1;
                        break;

                }

                //Still need to figure out how to set the sprite on creation - presumablly based on the attributes but how that works depends how many unique sprites we have 
                //create the potion; this needs more added to it to add the potion to your inventory, display ui, etc. 
                Potion pot = new Potion(name, image, duration, primary, secondary, mod);
                Debug.Log("name: " + name + "\n" + "p:" + primary + "\ns: " + secondary + "\nm: " + mod + "\nd: " + duration);
            } else {
                Debug.Log("Potion creation failed");
                //Debug.Log(sortedAtt[0].attribute.ToString() + " " + sortedAtt[0].count);
            }

            }
        }
}
