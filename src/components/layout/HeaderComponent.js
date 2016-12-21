import React from 'react';

require('../../styles/layout/Header.css');


let logo = require('../../logo.svg')

class HeaderComponent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isOpen: false
    }
  }

  get toggleOpen(){
    return () => this.setState({isOpen: !this.state.isOpen})
  }

  _getBrand(){
    return  <li>
      <div id="brand"><img src={logo} id="brand-logo" alt="brand logo"/><a href="#"><span>SounDark</span></a></div>
      {window.innerWidth > 800 ? '': <a className='btn-open' onClick={this.toggleOpen}>{this.state.isOpen? 'v':'>'}</a>}
  </li>
  }

  render() {
    return (
      <div className={`header-component ${this.props.isSticky ? 'sticky' : ''}`}>
        <ul className={this.state.isOpen? 'is-open' : 'is-closed'}>
          {this._getBrand()}
          {this.props.items.map(item =><li key={item.id}><a href={item.id}> {item.name}</a></li>)}
  </ul>
</div>)

  }



}

HeaderComponent.displayName = 'SectionsHeaderComponent';

// Uncomment properties you need
// HeaderComponent.propTypes = {};
// HeaderComponent.defaultProps = {};

export default HeaderComponent;
