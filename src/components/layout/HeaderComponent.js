'use strict';

import React from 'react';

require('styles/layout/Header.css');

class HeaderComponent extends React.Component {

  render() {
    return (
      <div className="header-component">
        <ul>
          {this.props.items.map(item => <li key={item.id}><a href={item.id}> {item.content}</a></li>)}
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
