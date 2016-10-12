'use strict';

import React from 'react';

require('styles/content/Intro.css');

const IntroComponent = (props) => (
      <div className="intro-component">
        <p>
          We wanted to create a game in which the player uses their voice to interact with and control the environment.
        </p>
        <p>
          Some animals, like bats, use sound as a primary tool for traversing the environment; this way of viewing the world became our main interaction paradigm, and one of our goals has been to make this feel natural using both graphical and auditive feedback.
        </p>
      </div>
)

IntroComponent.displayName = 'ContentIntroComponent';

// Uncomment properties you need
// IntroComponent.propTypes = {};
// IntroComponent.defaultProps = {};

export default IntroComponent;
