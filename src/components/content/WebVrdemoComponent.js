'use strict';

import React from 'react';

require('styles/content/WebVrdemo.css');

let screenshot = require('../../images/screenshot.jpg')

class WebVrdemoComponent extends React.Component {
  render() {
    return (
      <div className="webvrdemo-component">
        <a href="https://rodrigoroarodriguez.github.io/WebVRDemo/">
          <img src={screenshot} alt="screenshot" className="screenshot"/>
        </a>
      </div>
    );
  }
}

WebVrdemoComponent.displayName = 'ContentWebVRDemoComponent';

// Uncomment properties you need
// WebVrdemoComponent.propTypes = {};
// WebVrdemoComponent.defaultProps = {};

export default WebVrdemoComponent;
