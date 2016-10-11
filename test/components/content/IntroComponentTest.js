/* eslint-env node, mocha */
/* global expect */
/* eslint no-console: 0 */
'use strict';

// Uncomment the following lines to use the react test utilities
// import TestUtils from 'react-addons-test-utils';
import createComponent from 'helpers/shallowRenderHelper';

import IntroComponent from 'components/content/IntroComponent.js';

describe('IntroComponent', () => {
  let component;

  beforeEach(() => {
    component = createComponent(IntroComponent);
  });

  it('should have its component name as default className', () => {
    expect(component.props.className).to.equal('intro-component');
  });
});
