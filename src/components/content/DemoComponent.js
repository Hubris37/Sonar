'use strict';

import React from 'react';

require('styles/content/Demo.css');

//I didn't manage to get a screenshot of the game so I used the poster instead.
let screenshot = require('../../images/poster.jpg')

class DemoComponent extends React.Component {
  render() {
    return (
      <div className="webvrdemo-component">
        <img src={screenshot} alt="screenshot" className="screenshot"/>
      </div>
    );
  }
}

DemoComponent.displayName = 'ContentDemoComponent';

// Uncomment properties you need
// DemoComponent.propTypes = {};
// DemoComponent.defaultProps = {};

export default DemoComponent;
