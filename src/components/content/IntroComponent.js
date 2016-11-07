'use strict';

import React from 'react';

require('styles/content/Intro.css');

const IntroComponent = (props) => (
      <div className="intro-component">
        <p>What do you get when you combine virtual reality, multimodal interaction, asymmetric gameplay and a chicken? <em>SounDark</em> is what you get.</p>
        <p>SounDark, by team Hubris, is a thriller game set in a surreal environment where screaming is recommended. The player wearing the VR-headset plays as a blind chicken that can only see through <strong>echolocation</strong>, so staying silent in a maze filled with blood-thirsty cooks and deadly traps might not be in their best interest.</p>
        <p>On the other hand, spectators, who can see the map from an isometric perspective,  can choose to either help the player by providing them with information about the maze layout or play as a mask enemy that spits out balls that activate traps and clutter the way to the goal.</p>
      </div>
)

IntroComponent.displayName = 'ContentIntroComponent';

// Uncomment properties you need
// IntroComponent.propTypes = {};
// IntroComponent.defaultProps = {};

export default IntroComponent;
