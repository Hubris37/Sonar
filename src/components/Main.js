require('normalize.css/normalize.css');
require('styles/App.css');

import React from 'react';
import HeaderComponent from './sections/HeaderComponent'
import FooterComponent from './sections/FooterComponent'
import AboutComponent from './content/AboutComponent'
import DemoComponent from './content/DemoComponent'
import WebVRDemoComponent from './content/WebVRDemoComponent'
import IntroComponent from './content/IntroComponent'

let logo = require('../android-chrome-192x192.png')
let brand = {
  content: <div id="brand"><img src={logo} id="brand-logo" alt="brand logo"/>
    <span>SounDark</span>
  </div>,
  id: '#'
}

let sectionNames = [
  'Introduction',
  'WebVR',
  // 'The project',
  'Demo',
  'About'
]

let components = {
  'Introduction': <IntroComponent/>,
  'WebVR': <WebVRDemoComponent/>,
  'Demo': <DemoComponent/>,
  'About': <AboutComponent/>
}

let sections = sectionNames.map(section => ({
  content: section,
  id: '#' + section
}))

const sectionTemplate = section => <div className='section'>
  <h1 key={sections.id}>
    <a name={section.content}>{section.content}</a>
  </h1>
  <div className='section-content'>
    {components[section.content]}
  </div>
</div>

// {groupMembers.map(member=> <img src={member.image} key={member.name}  alt={member.name} />)}
class AppComponent extends React.Component {
  render() {
    return (
      <div className="index">
        <HeaderComponent items={[].concat(brand, sections)}/>
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
