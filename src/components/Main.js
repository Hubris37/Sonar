require('normalize.css/normalize.css');
require('styles/App.css');

import React from 'react';
import HeaderComponent from './layout/HeaderComponent'
import FooterComponent from './layout/FooterComponent'
import AboutComponent from './content/AboutComponent'
import ForskarFredag from './content/ForskarFredagComponent'
import {Introduction, Technology, Design, Testimonials} from './sections'

let landingImage = <img src={require('../images/landing-image.jpg')} id="landing-image" alt="SounDark landing image"/>

let components = {
  'Introduction': <Introduction/>,
  'Technology': <Technology/>,
  'Design': <Design/>,
  'Forskarfredag': <ForskarFredag/>,
  'Testimonials': <Testimonials/>,
  'About': <AboutComponent/>
}

let sections = Object.keys(components).map(section => ({
  name: section,
  id: '#' + section
}))


const sectionTemplate = section => <div className='section' key={section.id}>
  <h1>
    <a name={section.name}>{section.name}</a>
  </h1>
  <div className='section-content'>
    {components[section.name]}
  </div>
</div>

class AppComponent extends React.Component {

  constructor(props){
    super(props)
    this.state = {isHeaderSticky: false}
  }

  /** Makes header conditionally sticky. **/
  componentDidMount(){
    let aboveHeader = document.getElementById('landing-image')
    window.onscroll = ()=>{
      let isBelow = aboveHeader.getBoundingClientRect().bottom > 0
      let isSticky = this.state.isHeaderSticky
      // Header is sticky only if you have scrolled past it.
      if(isBelow === isSticky) this.setState({isHeaderSticky:!isSticky})
    }
  }

  /**
   * The layout of the page there is a landing image on top, a header and then
   * sections each one with a heading on top.
   */
  render() {
    return (
      <div className="index">
        {landingImage}
        <HeaderComponent
          isSticky = {this.state.isHeaderSticky}
          items={sections}/>
        <main>
        {sections.map(sectionTemplate)}
        </main>
        <FooterComponent/>
      </div>
    );
  }
}

AppComponent.defaultProps = {};

export default AppComponent;
