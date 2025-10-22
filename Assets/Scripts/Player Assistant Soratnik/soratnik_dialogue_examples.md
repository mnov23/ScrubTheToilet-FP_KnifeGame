# Soratnik Dialogue Examples

Here are some example dialogue lines for your comrade Soratnik based on his emotional states:

## Default (Happy)
- "Clean faster, comrade! Time is ticking!"
- "Good work! But you can do better!"
- "This is what you trained for!"

## Amazed
- "Wow! Look at that technique!"
- "Incredible! You're a natural!"
- "I've never seen anyone clean like that!"

## Neutral
- "Keep going."
- "Adequate."
- "Continue your work."

## Devilish
- "Hehe... enjoying yourself?"
- "What a glorious job you have, comrade!"
- "This is exactly where you belong..."

## Angry
- "FASTER! My grandmother cleans better!"
- "What are you doing?! MOVE!"
- "Pathetic! Put some effort into it!"

## Smirk
- "Missing your old job yet?"
- "Oh, did I mention the deadline?"
- "Hope you're not getting tired..."

## Superior (Looking Down)
- "I would never stoop to this level."
- "Remember your place, soldier."
- "Some are born leaders. Others... clean."

---

## Setup Instructions for Game.unity:

### 1. Create Soratnik Character UI Layer:
```
Canvas
└── SoratnikContainer (Image - optional background)
    ├── SoratnikBody (Image - full body sprite without face/hand)
    ├── SoratnikFace (Image - swappable face layer)
    ├── SoratnikHand (Image - swappable hand with tool)
    └── DialogueContainer
        ├── DialogueBox (Image - speech bubble background)
        └── DialogueText (Text - Soratnik's words)
```

### 2. Prepare Your Assets:
- **Body sprite**: Full Soratnik body (without face and right hand)
- **7 Face sprites**: One for each emotion
- **2 Hand sprites**: One holding fork, one holding katana
- **Audio clips**: Record your voice lines (one per dialogue)

### 3. GameManager Inspector Setup:

**Soratnik Character:**
- Drag SoratnikBody → Soratnik Body Image
- Drag SoratnikFace → Soratnik Face Image
- Drag SoratnikHand → Soratnik Hand Image
- Set Emotions array size to 7:
  - [0] Name: "default" | Face Sprite: (happy face)
  - [1] Name: "amazed" | Face Sprite: (amazed face)
  - [2] Name: "neutral" | Face Sprite: (neutral face)
  - [3] Name: "devilish" | Face Sprite: (devil face)
  - [4] Name: "angry" | Face Sprite: (angry face)
  - [5] Name: "smirk" | Face Sprite: (smirk face)
  - [6] Name: "superior" | Face Sprite: (superior face)
- Drag fork hand sprite → Fork Hand Sprite
- Drag katana hand sprite → Katana Hand Sprite

**Soratnik Dialogue:**
- Drag DialogueText → Dialogue Text
- Drag DialogueBox → Dialogue Box
- Add AudioSource component → Drag to Soratnik Voice Source
- Set Random Dialogues array size (as many lines as you want)
- For each dialogue:
  - Write text
  - Drag voice audio clip
  - Set emotion state (exact name from emotions array)
  - Set display duration (3-5 seconds recommended)
- Min Dialogue Interval: 10 seconds
- Max Dialogue Interval: 20 seconds

### 4. Art Setup Tips:

**Layering in Photoshop/Your 2D software:**
1. Create base body layer (soldier uniform, no face, no right hand)
2. Create 7 face variations on separate layers
3. Create 2 hand variations (fork, katana)
4. Export each as separate PNG with transparency
5. Make sure face and hand positions align perfectly with body

**Sprite Import Settings in Unity:**
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Pixels Per Unit: 100 (adjust based on your art size)
- Filter Mode: Bilinear
- Compression: None (for crisp 2D art)

### 5. Voice Recording Tips:
- Record in a quiet room
- Use consistent volume across all lines
- Add slight Russian accent if you want authenticity
- Keep lines short (3-5 seconds max)
- Export as .mp3 or .wav
- Unity accepts both, but .wav is higher quality

### 6. Position Soratnik:
- Place him on the left or right side of screen
- Make sure he doesn't block the toilet (main gameplay area)
- DialogueBox should appear near his head
- Use anchors to make UI responsive for different screen sizes