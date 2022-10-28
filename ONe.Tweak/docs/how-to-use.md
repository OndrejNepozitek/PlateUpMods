# How to use (ONe.Tweak)

This page contains information regarding how to use and configure the **ONe.Tweak** mod. Make sure that you install the mod first.

## Read first

### Warning: Do not install the mod in the middle of your best run

Every software has bugs. Make sure that you try the mod with a run that you do not mind losing. I tried my best testing the mod but you can never be 100% sure.

### Warning: Some tweaks are enabled by default

I decided to enable some tweaks by default to improve the experience of using the mod. For example, the *RestartAnyDay* feature of the [Casual mode](#casual-mode) is enabled by default to make sure that you do not accidentally lose your run thinking that it is enabled. Feel free configure the mod any way you like.

### How to configure

It is recommended to use the Configuration Manager (press F1 when in game) to configure the mod. Inside the Configuration Manager window, you have to find the *ONe.Tweak* section and click on it. You can also find the configuration file in `<game folder>\BepInEx\config\ONe.Tweak.cfg`.

### How to configure keyboard shortcuts

Some features in the mod are actions that you need to trigger manually. For example, if you want to start the practice mode from anywhere. For each such action, you have two options of triggering it. If you open the Configuration Manager window (F1), find the row for the action that you want to trigger. You can click the *Run* button to trigger the action once. Or you can click on the button next to it, which will prompt you for a keyboard shortcut that will trigger the action.

## Tweak: Separate seeds

In vanilla PlateUp!, the map seed determines both the layout of the restaurant and the blueprints, cards, etc. that you get when playing. That means that the several most popular seeds are played again and again because people like the layout. But sometimes, you might want to keep the layout but have a different seed for everything else. That is precisely what this tweak does. When enabled, after you enter your desired seed, the game generates the layout and then will randomly pick a different seed with which you will be playing. Therefore, you can play the same layout repeatedly and always get something new.

Moreover, you can also replay your session with the same separate seed. Just set *UseRandomSeed* to false and put your desired seed in the *FixedSeed* option.

In my opinion, this is a very vanilla-like way to make the good old seeds new again.

### Multiplayer

In multiplayer sessions, only the host needs to have this feature enabled.

### Config format

It is recommended to use the Configuration Manager (press F1 when in game) to configure the mod. Inside the Configuration Manager window, you have to find the *ONe.Tweak* section and click on it. You can also find the configuration file in `<game folder>\BepInEx\config\ONe.Tweak.cfg`.

```ini
## Whether to use one seed for layout generation and a different seed for everything else.
# Setting type: Boolean
# Default value: false
Enabled = false

## Whether the other seed should be randomly generated or not.
# Setting type: Boolean
# Default value: true
UseRandomSeed = true

## If UseRandomSeed is set to false, this seed will be used as the other seed.
# Setting type: String
# Default value: 
FixedSeed = 
```


## Tweak: Ghost mode

Do you feel like you spend more time in preparation phase just moving things around than actually playing the game? Then our *Ghost mode* might be a good choice for you. In *Ghost mode*, you can move through objects, which greatly simplifies the preparation phase.

To enable this feature, I recommend enabling the *EnableOnPreparationStart* option, which will automatically turn on the *Ghost mode* when the preparation phase starts. You can also use the *ToggleGhostModeKeyboardShortcut* to toggle the *Ghost mode* on key press.

### Multiplayer

If you are playing multiplayer over the network, it is recommended that all players have the mod installed and have it configured the same way. Otherwise, the *Ghost mode* may behave weirdly.

### Config format

It is recommended to use the Configuration Manager (press F1 when in game) to configure the mod. Inside the Configuration Manager window, you have to find the *ONe.Tweak* section and click on it. You can also find the configuration file in `<game folder>\BepInEx\config\ONe.Tweak.cfg`.

```ini
## Whether to enable ghost mode at the start of each day's preparation phase.
# Setting type: Boolean
# Default value: false
EnableOnPreparationStart = false

## Whether to disable ghost mode at the end of each day's preparation phase.
# Setting type: Boolean
# Default value: true
DisableOnPreparationEnd = true

## Whether to resize map bounds when in ghost mode. Otherwise, the game teleports you to the front door if you leave the walls.
# Setting type: Boolean
# Default value: true
ResizeBoundsWhenEnabled = true

## Keyboard shortcut for toggling the ghost mode.
# Setting type: KeyboardShortcut
# Default value:
ToggleGhostModeKeyboardShortcut = 
```

### Multiplayer

In multiplayer sessions, this feature must be enabled on the host's side and only the host can start the preparation phase via the shortcut/button. Other clients do not need to have the mod installed for this feature to work.

### Config format

It is recommended to use the Configuration Manager (press F1 when in game) to configure the mod. Inside the Configuration Manager window, you have to find the *ONe.Tweak* section and click on it. You can also find the configuration file in `<game folder>\BepInEx\config\ONe.Tweak.cfg`.

```ini
## Whether to restore player positions after exiting the practice mode.
# Setting type: Boolean
# Default value: true
RestorePositionsAfterPractice = true

## Keyboard shortcut for starting the practice mode from anywhere.
# Setting type: KeyboardShortcut
# Default value:
StartPracticeKeyboardShortcut = 
```

## Tweak: Casual mode

If you are looking for a more casual experience while playing PlateUp!, you can enable the *RestartAnyDay* feature which makes it possible to restart any day after your customers' experience runs out. No more losing a run because you forgot to set a smart grabber. It is up to you do decide when you deserve to lose the run.

*Tip*: The restart loads the last saved state, which is usually the start of the previous preparation phase. However, in the current version of the game, you can open the *Start practice mode* popup to force the game to create a new save.

### Multiplayer

In multiplayer sessions, this feature must be enabled on the host's side.

### Config format

It is recommended to use the Configuration Manager (press F1 when in game) to configure the mod. Inside the Configuration Manager window, you have to find the *ONe.Tweak* section and click on it. You can also find the configuration file in `<game folder>\BepInEx\config\ONe.Tweak.cfg`.

```ini
## Whether any day can be restarted the same way as the first 3 days.
# Setting type: Boolean
# Default value: true
RestartAnyDay = true
```

## Tweak: Creative mode (Experimental)

In order to test some features of this mod, I needed to be able to skip a day. The *SkipDay* feature does exactly that. If you start a new day and trigger it, it will skip to the end of the day. If you trigger without starting the next day, it will replay the the end of the previous day.

### Multiplayer

In multiplayer sessions, this feature must be enabled on the host's side.

### Config format

It is recommended to use the Configuration Manager (press F1 when in game) to configure the mod. Inside the Configuration Manager window, you have to find the *ONe.Tweak* section and click on it. You can also find the configuration file in `<game folder>\BepInEx\config\ONe.Tweak.cfg`.

```ini
## Skip the current day
# Setting type: KeyboardShortcut
# Default value: 
SkipDayKeyboardShortcut = 
```

## Tweak: Cards (Experimental)

I wanted to try some franchise runs but did not feel like playing 15 day just to choose from 2 cards that I did not like. Therefore, I tried to come up with a solution that is as vanilla-like as possible. In the end, I decided to make it possible to choose from more cards. For example, you can configure the mod to give you 2 additional cards (4 total) when you reach level 15.

The game is programmed to only show 2 cards at a time. If you want to see the additional cards, you will need to trigger the *ShowMoreCards* action either via the *Configuration Manager* or by [assigning a keyboard shortcut to it](#how-to-configure-keyboard-shortcuts). By triggering the action repeatedly, you will cycle through all the available cards.

### Warning: Twitch integration

This feature might not work well with the Twitch integration. 

### Tweak: Multiplayer

In multiplayer sessions, this feature must be enabled on the host's side.

### Config format

It is recommended to use the Configuration Manager (press F1 when in game) to configure the mod. Inside the Configuration Manager window, you have to find the *ONe.Tweak* section and click on it. You can also find the configuration file in `<game folder>\BepInEx\config\ONe.Tweak.cfg`.

```ini
## Whether choose from additional customer, dish and franchise card.
# Setting type: Boolean
# Default value: false
EnableAdditionalCards = false

## How many additional customer/dish cards to choose from.
# Setting type: Int32
# Default value: 0
# Acceptable value range: From 0 to 10
AdditionalCustomerDishCards = 0

## How many additional franchise cards to choose from.
# Setting type: Int32
# Default value: 2
# Acceptable value range: From 0 to 10
AdditionalFranchiseCards = 2

## Cycle through all available cards.
# Setting type: KeyboardShortcut
# Default value: 
ShowNextCardKeyboardShortcut = 
```