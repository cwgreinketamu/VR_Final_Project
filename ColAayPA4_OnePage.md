# Assignment 4: Haptics Room
**Cole Greinke and Aayush Upadhyay**
**VIST 477 Virtual Reality, Spring 2026**

---

## Task Overview and Objectives

The Haptics Room is a "Waterfall Grotto" wing of the museum, a cave-like environment with colored lighting, water streams, glowing crystals, and a dark alcove. The player completes three haptic-driven challenges: identifying the strongest water stream by vibration alone, finding objects by texture feel in a dark alcove, and hunting for hidden crystals using proximity-based haptic guidance. The room is designed so that haptic feedback is the primary channel for completing objectives, not just supplementary to visuals.

## Haptic Techniques and Rationale

We implemented six distinct haptic patterns, each varying amplitude, duration, and controller source to serve different communicative purposes:

**Confirmation** uses a double-pulse pattern (two quick 0.6 amplitude, 0.08s pulses with a short gap). It fires on painting discovery, crystal collection, and correct answers. The double-tap pattern is chosen because it feels deliberately different from a single environmental bump.

**Guidance** provides continuous proximity-based vibration during the crystal hunt. Amplitude scales from 0.05 (far) to 0.8 (near), and pulse interval shrinks from 0.6s to 0.08s as the player approaches, creating a "getting warmer" effect through touch alone.

**Warning/Failure** uses a rapid triple-buzz (three 0.9 amplitude, 0.05s pulses in quick succession). This pattern is perceptually distinct from confirmation because it's faster, stronger, and has three hits instead of two. It fires on wrong answers, scrambler pickup, and wrong sorting in the manipulation room.

**Directional Feedback** uses left/right controller independently during crystal hunt. The left controller vibrates when the target is to the player's left, right controller when it's to the right, and both vibrate when facing the target. This communicates spatial information through the haptic channel.

**Multi-Modal Feedback** synchronizes haptics with audio and visuals. Water stream entry triggers both a vibration pattern and water audio simultaneously. Crystal collection pairs a haptic confirmation with a visual glow increase. The synchronization reinforces each modality.

**Texture/Weight Simulation (Bonus)** uses continuous haptic patterns to simulate material properties for the blind identification challenge. Smooth objects produce a gentle steady vibration (0.2 amplitude). Rough objects produce rapid randomized micro-vibrations (0.3-0.6 amplitude, irregular timing). Heavy objects produce slow strong pulsing (0.7 amplitude at ~3Hz). These patterns are deliberately distinct enough that players can tell them apart without seeing the objects.

## Haptic Objectives Description

1. **Water Streams (Confirmation + Multi-Modal):** Three streams with gentle, medium, and strong haptic patterns. Player feels each, then selects the strongest. Correct answer triggers confirmation; wrong triggers warning.
2. **Blind Identification (Texture + Warning):** Three objects with different haptic textures in a dark alcove. Player must identify the prompted texture type by touch. Three rounds with different prompts.
3. **Crystal Hunt (Guidance + Directional):** Four crystals hidden around the cave. Proximity-based vibration guides the player, with directional left/right feedback indicating which way to turn. Collection triggers multi-modal confirmation.

## Success/Failure Conditions

Success requires completing all three phases within a 3-minute time limit. Phase 1 requires identifying the strongest stream. Phase 2 requires correctly identifying 3 textures. Phase 3 requires collecting all 4 crystals using haptic guidance. Failure occurs if the timer expires or the player gives 3 wrong answers across all phases. Success feedback is a strong sustained pulse with green UI text. Failure is the warning triple-buzz with red text.

## Haptic Additions to Previous Rooms

**Navigation Room:** Painting discovery now triggers a confirmation double-pulse synchronized with the existing chime sound. The scrambler collectible triggers a warning triple-buzz on pickup, reinforcing the "something bad happened" feeling through touch.

**Manipulation Room:** Correct statue sorting triggers the confirmation pattern, while incorrect sorting triggers the warning pattern. This gives immediate tactile feedback on whether the throw landed in the right zone, which is especially useful when the statues go over the wall and the player can't always see where they landed.

## Changes from Assignment 1

The original design described a waterfall with position-dependent haptic intensity, bioluminescent moss interaction, and a blind identification challenge. We kept the core concepts (water streams with varying haptic intensity, blind texture identification, crystal interaction) but simplified the environmental complexity. Instead of a fully modeled cave with waterfall particles and bioluminescent moss, we built the grotto from primitives with emissive crystal materials and colored point lights. The haptic challenge became a structured three-phase game with clear progression rather than a freeform exploration.

## Technical Insights

All haptic output goes through a central `HapticManager` singleton that holds references to both controller `HapticImpulsePlayer` components. This avoids every script needing its own controller references. Haptic patterns (confirmation double-pulse, warning triple-buzz) are implemented as coroutines so they can sequence multiple impulses with precise timing gaps. The texture simulation uses randomized amplitude and timing for the "rough" pattern, which produces irregular vibrations that feel qualitatively different from the smooth steady pulse. The proximity guidance system recalculates distance every frame but only sends impulses at a variable rate based on distance, preventing haptic fatigue while still conveying approach speed. Directional feedback computes the dot product between the player's right vector and the direction to the target to determine which controller should vibrate. Testing haptics in the XR Device Simulator is inherently limited since there's no physical vibration, so we validated the code structure and timing through debug logs and tested final feel on the Quest hardware.
