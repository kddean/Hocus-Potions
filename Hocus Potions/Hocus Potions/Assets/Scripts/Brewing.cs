using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brewing : MonoBehaviour {


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
        Sprite image;
        string name = "";

        for(int i = 0; i < 9; i++) {
            if(System.Enum.IsDefined(typeof(Ingredient.Modifiers), attArray[i].ToString())) {
                modifiers.Add(new modList(1, (Ingredient.Modifiers)System.Enum.Parse(typeof(Ingredient.Modifiers), attArray[i].ToString())));
            } else {
                attributes.Add(new attList(1, attArray[i]));
            }


        }

        List<attList> sortedAtt = attributes.OrderBy(attList => attList.count).ToList();
        List<modList> sortedMod = modifiers.OrderBy(modList => modList.count).ToList();

        if (sortedAtt.Count() == 0) {
            Debug.Log("Error, no attributes");
        } else {
                if (sortedAtt[0].count >= 3) {
                    //accounts for ties in primary attribute
                    if (sortedAtt[0].count == sortedAtt[1].count) {
                        int flip = Random.Range(0, 2);
                        if (flip == 0) {
                            primary = sortedAtt[0].attribute;
                        } else {
                            primary = sortedAtt[1].attribute;
                        }
                        //if it's not a tie just use the first attribute
                    } else {
                        primary = sortedAtt[0].attribute;
                    }

                    if (sortedAtt.Count() == 1) {
                        secondary = null;
                    } else {
                        if (sortedAtt[1].count >= 2) {
                            //accounts for ties in secondary attribute
                            if (sortedAtt.Count() > 2 && sortedAtt[1].count == sortedAtt[2].count) {
                                int flip = Random.Range(0, 2);
                                if (flip == 0) {
                                    secondary = sortedAtt[1].attribute;
                                } else {
                                    secondary = sortedAtt[2].attribute;
                                }
                                //if it's not a tie just use the second attribute
                            } else {
                                secondary = sortedAtt[1].attribute;
                            }
                        } else {
                            secondary = null;
                        }
                    }

                //Modifiers - *need to talk to Mikaila about the logic because it's missing a bunch of edge cases but this should be functional at least*
                if (sortedMod.Count < 1) {
                    mod = null;
                } else {
                    if (primary == Ingredient.Attributes.transformation || secondary == Ingredient.Attributes.transformation) {
                        switch (sortedMod[0].attribute) {
                            case Ingredient.Modifiers.cat:
                            case Ingredient.Modifiers.chicken:
                            case Ingredient.Modifiers.sheep:
                                mod = sortedMod[0].attribute;
                                break;
                            default:
                                mod = null;
                                break;
                        }
                    } else {
                        switch (sortedMod[0].attribute) {
                            case Ingredient.Modifiers.magicMM:
                            case Ingredient.Modifiers.magicPP:
                                mod = sortedMod[0].attribute;
                                break;
                            default:
                                mod = null;
                                break;
                        }
                    }
                       
                    }


                //Potion name generation *Rough Draft just for functionality; names not final*
                name = primary.ToString() + "potion of " + secondary.ToString();
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

                //Duration calculation *Default values atm, needs to include strength adjustments based on count*
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
                } else {
                    Debug.Log("Potion creation failed");
                }

            }
        }
  

    }
}
