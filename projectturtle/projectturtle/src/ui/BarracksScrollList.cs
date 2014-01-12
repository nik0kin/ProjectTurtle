using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.util;
using ProjectTurtle.src.objects.playerclasses.heroes;
using ProjectTurtle.src.objects;
using ProjectTurtle.src.objects.playerclasses;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.screens;

//item switchout occurs here too
//click item button to get to an inventory screen, which also has a reset all items button

namespace ProjectTurtle.src.ui {
    class BarracksScrollList {
        static Texture champBarTexture;
        static Texture assa, arch, puri, vang, pyro, bard;

        Vector2f start;
        FloatRect overallRect = new FloatRect(0, 0, 200, 420);
        FloatRect champBarRect = new FloatRect(0, 0, 200, 50);
        FloatRect itemIconRect = new FloatRect(0, 0, 20, 20);
        Vector2f itemIconStart = new Vector2f(169,15);//onchamp bar

        Sprite champBarSprite, unitIconSprite;
        Text topBarDebugText, playerClassText, playerItemText;

        const int scroll = 0;//from top and bottom
        int totalBars;
        int topBar = 0;
        int maxShown;//y space / champbarY

        Barracks mBarracks;
        //List<HeroPair> champs;
        Inventory inventory;//inventory screen
        ItemID invResult;//inventory result
        int changingChamp;

        Vector2f lastClicked;
        ChampItemPair? draggedChamp;

		List<int> disabledButtons;//int is number according to the array in barracks.getPeoples
        List<IconToolTip> champsItemIcons;
        
        internal static void LoadContent(){
            champBarTexture = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barracksunit");//300,60

            assa = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackassa");
            arch = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackarch");
            puri = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackpuri");
            vang = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackvang");
            pyro = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackpyro");
            bard = GameBox.loadTexture(RaidSelect2.IMAGEPATH + "barrackbard");
        }
        //totalbars - maxShown = max topBar

        internal BarracksScrollList(Barracks b, Vector2f start) {
            //int x = (overallRect.Height - 2 * 25) / champBarRect.Height;
            //scroll = overallRect.Height - (x * champBarRect.Height);
            maxShown = (int)((overallRect.Height - 2 * scroll) / champBarRect.Height - 1);
            totalBars = b.getTotalChamps();
            mBarracks = b;
            this.start = start;

			disabledButtons = new List<int>();
            champsItemIcons = new List<IconToolTip>();
            for(int i=0;i<totalBars;i++){
                ItemID item = mBarracks.getChamps()[i].item;
                Sprite itemS = new Sprite(Item.getItemTexture(item), new IntRect(0, 0, Item.ICON_WIDTH, Item.ICON_WIDTH));
                if (item == ItemID.none) {
                    IconToolTip itt = new IconToolTip(itemS, "none", "none");
                    champsItemIcons.Add(itt);
                } else {
                    Item ite = new Item(item);
                    champsItemIcons.Add(new IconToolTip(itemS, ite.name, ite.description));
                }
            }

            champBarSprite = new Sprite(champBarTexture);
            unitIconSprite = new Sprite();

            topBarDebugText = new Text(topBar + " ",GameBox.corbalFont,30U);
            topBarDebugText.Color = Color.Yellow;
            playerClassText = new Text("playerclass", GameBox.corbalFont, 15U);
            playerItemText = new Text("playeritem", GameBox.corbalFont, 15U);
        }

        internal void draw(RenderWindow window) {
            if (inventory != null){
                inventory.draw(window);
                return;
            }
            
            List<ChampItemPair> champs = mBarracks.getChamps();
            for (int i = 0; i < maxShown;i++ ) {
                drawChampBar(window, champs[topBar + i], champsItemIcons[topBar + i], new Vector2f(start.X, start.Y + champBarRect.Height * i + scroll), disabledButtons.Contains(i + topBar));
            }
            //then draw tooltips
            for (int i = 0; i < maxShown; i++) {
                champsItemIcons[topBar + i].drawToolTip(window);
            }
            window.Draw(topBarDebugText);
        }
        //basic scroll right now
        internal void update(double gameTime, SimpleMouseState mouseState) {
            
            if (inventory != null && (invResult = inventory.update(mouseState)) != ItemID.na) {
                //if the inventory returns an item
                inventory = null;
                mBarracks.swapItems(changingChamp, invResult);
                return;
            }
            int mouseY = mouseState.getPos().Y;
            if (GeoLib.isInRect(start,overallRect, new Vector2f(mouseState.getPos().X, mouseState.getPos().Y))){//start.X <= mouseState.getPos().X && mouseState.getPos().X <= start.X + overallRect.Width) {
                if (start.Y <= mouseY && mouseY <= start.Y + scroll) {
                    //clicked in the top box
                    if (mouseState.DoneReleasedIt(mouseButton.LEFT)) {
                        backButton();
                    }
                }else if(start.Y + overallRect.Height - scroll <= mouseY && mouseY <= start.Y + overallRect.Height){
                    //bottom box
                    if (mouseState.DoneReleasedIt(mouseButton.LEFT)){
                        nextButton();
                    }
                } else if (start.Y + scroll <= mouseY && mouseY <= start.Y + overallRect.Height - scroll) {
                    //somewhere in the middle
                    if (draggedChamp != null && mouseState.getPos().X >= start.X + itemIconStart.X) {//if on right side to hover over icons
                        int hoveredOver = -1;//TODO this could be nicer
                        hoveredOver = (int)(topBar + (mouseState.getPos().Y - (int)start.Y - scroll) / (champBarRect.Height));

                        if (mouseState.DoneReleasedIt(mouseButton.LEFT))
                            clickedChampItem(hoveredOver);
                    }
                    //do drag logic
                    if (mouseState.DoneClickedIt(mouseButton.LEFT)) {
                        Vector2f current = new Vector2f(mouseState.getPos().X, mouseState.getPos().Y);
                        draggedChamp = mBarracks.getChamps()[(int)(topBar + (mouseState.getPos().Y - (int)start.Y - scroll) / (champBarRect.Height))];
                        lastClicked = current;
                    } else if (!mouseState.left) {
                        draggedChamp = null;
                    }
                    //do scroll logic
                    if (mouseState.mouseWheelPos - mouseState.prevMouseWheelPos >= 1) {
                        backButton();
                    }
                    if (mouseState.mouseWheelPos - mouseState.prevMouseWheelPos <= -1) {
                        nextButton();
                    } 
                }
                for (int i = 0; i < maxShown; i++) {
                    ItemID item = mBarracks.getChamps()[topBar+ i].item;
                    champsItemIcons[topBar+ i].update(mouseState,
                        new Vector2f(start.X + itemIconStart.X, start.Y + itemIconStart.Y + champBarRect.Height * i + scroll),item);
                }
            }
            if (!mouseState.left)//released
                draggedChamp = null;
             
        }
        internal void nextButton() {
            topBar++;
            if (topBar > totalBars - maxShown) topBar = totalBars - maxShown;
        }
        internal void backButton() {
            topBar--;
            if (topBar < 0) topBar = 0; 
        }
        internal void clickedChampItem(int num) {
            if (num == -1) throw new Exception("you clicked on nothing?");
            changingChamp = num;
            if(mBarracks.getTotalSpareItems() > 0)//later we'll display a message TODO
                inventory = new Inventory(mBarracks);
        }

        internal void drawChampBar(RenderWindow window, ChampItemPair champ,IconToolTip toolTip, Vector2f start, bool disabled) {
            Texture unitIcon = null;
            switch(champ.hero){
                case PlayerClassNum.vang:
                    unitIcon = vang; break;
                case PlayerClassNum.assa:
                    unitIcon = assa; break;
                case PlayerClassNum.mage:
                    unitIcon = pyro ;break;
                case PlayerClassNum.puri:
                    unitIcon = puri; break;
                case PlayerClassNum.archer:
                    unitIcon = arch; break;
                case PlayerClassNum.bard:
                    unitIcon = bard; break;
            }
            champBarSprite.Position = start;
            window.Draw(champBarSprite);
            if (unitIcon != null) {
                unitIconSprite.Texture = unitIcon;
                unitIconSprite.Position = new Vector2f(start.X + 8, start.Y);
                window.Draw(unitIconSprite);
            }
            //spriteBatch.Draw(Item.getItemTexture(champ.item), new Vector2f(start.X + itemIconStart.X, start.Y + itemIconStart.Y), Color.White);
            //new icondraw
            toolTip.draw(window);
            playerClassText.DisplayedString = PlayerClassI.getClassName(champ.hero).ToUpper();
            playerClassText.Position = new Vector2f(start.X + 57, start.Y + 5);
            playerClassText.Color = disabled ? new Color(128,128,128) : Color.White;
            window.Draw(playerClassText);
            playerItemText.DisplayedString = Item.getItemName(champ.item);
            playerItemText.Position = new Vector2f(start.X + 57, start.Y + 28);
            playerItemText.Color = disabled ? new Color(128, 128, 128) : Color.White;
            window.Draw(playerItemText);
        }
        internal ChampItemPair? checkHeroDrag() {
            //could only consider it dragged, if its a certain distance away from the click origin, or outsite the champbar box
			if(draggedChamp == null || disabledButtons.Contains(getChampIndex(draggedChamp,false))) return null;//add we cant dragg something thats used
            return draggedChamp;
        }
		//maybe we should use index's throughout this class instead of ChampItemPair, than have a 
		// getChampAtIndex(), it might be a little messy but its only for the raid select screen
		internal void setDisabledChamp(ChampItemPair champ, bool disabled){
			int champIndex = getChampIndex(champ, disabled);
			
			if(disabled){
				disabledButtons.Add(champIndex);
			}else{
				disabledButtons.Remove(champIndex);
			}
            return;
		}
		//if careForDisabled is true, it returns the first non disabled index
		//if its false it returns the first index;
		internal int getChampIndex(ChampItemPair? champN, bool careForDisabled){
            if (!champN.HasValue) throw new Exception("null champ in getChampIndex()");//TODO should this be nullable?
            ChampItemPair champ = champN.Value;

			int champIndex = -1;
			//we need to find champ index firs
			List<ChampItemPair> champs = mBarracks.getChamps();
			for(int i=0;i<champs.Count;i++){
				if(champ.id == champs[i].id){
					champIndex = i;
					break;	
				}
			}
            if (champIndex == -1)
                throw new Exception();
			return champIndex;
		}
        internal void clear() {
            disabledButtons = new List<int>();
        }
    }

    //silly quick inventory class
    class Inventory {
        //bool selected;
        List<ItemID> items;
        int cols, rows;
        List<Sprite> sprites;
        internal Inventory(Barracks barracks){
            items = barracks.getSpareItems();
            sprites = new List<Sprite>();
            sprites.Add(new Sprite(Item.getItemTexture(ItemID.none)));//for no texture?
            //items.Add(ItemID.none); add no changes button correctly
            rows = 1;
            cols = 1;
            int amt = items.Count;//pressing the first box is equiping nothing
            for (int i = 1; i < amt; i++) {
                double d = ((double)GameBox.GAMERESX / cols) / (GameBox.GAMERESY / rows);//get the ratio of x to y

                if(i == rows*cols){//full
                    if (d >= 4.0 / 3) {
                        cols++;
                    } else {
                        rows++;
                    }
                }
                sprites.Add(new Sprite(Item.getItemTexture(items[i])));
            }
        }
        internal ItemID update(SimpleMouseState mouseState) {
            if (mouseState.DoneReleasedIt(mouseButton.LEFT)) {
                if (mouseState.getPos().Y >= 0 && mouseState.getPos().Y <= 600 && mouseState.getPos().X >= 0 && mouseState.getPos().X <= 800){
                    int rowC = mouseState.getPos().Y / (GameBox.GAMERESY / cols);
                    int colC = mouseState.getPos().X / (GameBox.GAMERESX / rows);
                    if (rowC * rows + colC >= items.Count) return ItemID.none;
                    return items[rowC * rows + colC];
                }
            }


            //if (selected) ;

            return ItemID.na;
        }
        internal void draw(RenderWindow window) {
            //4:3 ratio?
            int i=0;
            for (int y = 0;y<rows ; y++) {
                for (int x = 0; x<cols; x++) {
                    sprites[i].Position = new Vector2f(x * (GameBox.GAMERESX / cols), y * (GameBox.GAMERESY / rows));
                    sprites[i].Scale = new Vector2f((GameBox.GAMERESX / cols) / sprites[i].TextureRect.Width, 
                            (GameBox.GAMERESY / rows) / sprites[i].TextureRect.Height);
                    window.Draw(sprites[i]);
                    i++;
                    if (i >= items.Count)
                        return;
                }
                
                
            }
            
        }
    }
}
