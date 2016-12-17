'use strict';

import React from 'react';

require('../../styles/layout/Header.css');


let logo = require('../../logo.svg')
let brand = {
  name: <div id="brand"><img src={logo} id="brand-logo" alt="brand logo"/>
    <span>SounDark</span>
  </div>,
  id: '#'
}

class HeaderComponent extends React.Component {
  render() {
    let items = [].concat(brand,this.props.items)
    return (
      <div className={`header-component ${this.props.isSticky ? 'sticky' : ''}`}>
        <ul>
          {items.map(item => <li key={item.id}><a href={item.id}> {item.name}</a></li>)}
        </ul>
      </div>
    );
  }
}

HeaderComponent.displayName = 'SectionsHeaderComponent';

// Uncomment properties you need
// HeaderComponent.propTypes = {};
// HeaderComponent.defaultProps = {};

export default HeaderComponent;
