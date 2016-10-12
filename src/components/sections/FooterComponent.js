'use strict';

import React from 'react';

require('styles/sections/Footer.css');

let repos = [{
    name:'WebVRDemo',
    url:'https://github.com/RodrigoRoaRodriguez/WebVRDemo'
  },{
    name:'Demo',
    url:'https://github.com/Hubris37/Sonar'
  },{
    name:'Webpage',
    url:'https://github.com/Hubris37/Sonar/tree/gh-pages'
  }]

class FooterComponent extends React.Component {
  render() {
    return (
      <div className="footer-component" id="footer">
        <div>
        <h3>Source code available at GitHub:</h3>
          <ul>
            {repos.map(repo => <li key={repo.name}> <a href={repo.url}> {repo.name}</a></li>)}
          </ul>
        <p id='lastModified'>This page was last modified: {document.lastModified}</p>
        </div>
      </div>
    );
  }
}

FooterComponent.displayName = 'SectionsFooterComponent';

// Uncomment properties you need
// FooterComponent.propTypes = {};
// FooterComponent.defaultProps = {};

export default FooterComponent;
