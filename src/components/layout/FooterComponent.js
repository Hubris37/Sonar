'use strict';

import React from 'react';

require('../../styles/layout/Footer.css');

let repos = [{
    name:'Source',
    url:'https://github.com/Hubris37/Sonar'
  },{
    name:'Webpage Source',
    url:'https://github.com/Hubris37/Sonar/tree/webpage-source'
  },
]

class FooterComponent extends React.Component {
  render() {
    return (
      <div className="footer-component" id="footer">
        <div>
        <h3>Source code available at GitHub:</h3>
          <ul>
            {repos.map(repo => <li key={repo.name}> <a href={repo.url}> {repo.name}</a></li>)}
          </ul>
        <p id='last-modified'>This page was last modified: {document.lastModified}</p>
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
