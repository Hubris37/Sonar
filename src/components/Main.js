require('normalize.css/normalize.css');
require('styles/App.css');

import React from 'react';
import HeaderComponent from './sections/HeaderComponent'
import FooterComponent from './sections/FooterComponent'
import AboutComponent from './content/AboutComponent'
import DemoComponent from './content/DemoComponent'
import WebVRDemoComponent from './content/WebVRDemoComponent'


let logo = require('../android-chrome-192x192.png')
let brand =  {
    content: <div id="brand"><img src={logo} id="brand-logo" alt="brand logo" /><span>SounDark</span></div>,
    id: '#'
}

let sectionNames = [
  // 'Introduction',
  'WebVRDemo',
  // 'The project',
  'Demo',
  'About'
]

let components = {
  'WebVRDemo' : <WebVRDemoComponent />,
  'Demo' : <DemoComponent />,
  'About' : <AboutComponent />
}

let sections = sectionNames.map(section=> ({content:section, id:'#'+section}))

// {groupMembers.map(member=> <img src={member.image} key={member.name}  alt={member.name} />)}
class AppComponent extends React.Component {
  render() {
    console.log([].concat(brand,sections))
    return (
      <div className="index">
        <HeaderComponent items={[].concat(brand,sections)}/>
        {sections.map(section=><div className='section'>
          <h1 key={sections.id}><a  name={section.content}>{section.content}</a></h1>
          <div className='section-content'>
            {components[section.content]}
          </div>
        </div>
        )}
        <FooterComponent />
      </div>
          );
        }
}

AppComponent.defaultProps = {
};

export default AppComponent;
