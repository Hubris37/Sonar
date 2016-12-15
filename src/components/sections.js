import React from 'react';

require('styles/sections.css');

const Introduction = (props) => (
      <div className="introduction">
        <p>What do you get when you combine virtual reality, multimodal interaction, asymmetric gameplay and a chicken? <em>SounDark</em> is what you get.</p>
        <p>SounDark, by team Hubris, is a thriller game set in a surreal environment where screaming is recommended. The player wearing the VR-headset plays as a blind chicken that can only see through <strong>echolocation</strong>, so staying silent in a maze filled with blood-thirsty cooks and deadly traps might not be in their best interest.</p>
        <p>On the other hand, spectators, who can see the map from an isometric perspective,  can choose to either help the player by providing them with information about the maze layout or play as a mask enemy that spits out balls that activate traps and clutter the way to the goal.</p>
        <h2 id="goals-and-motivation">Goals and motivation</h2>
        <p>SounDark seeks to create an immersive experience different from anything already out there. The idea is to create a perceptual experience feel alien to the player as a human being but still feels simple and intuitive.</p>
      </div>
)

const Technology = (props) => (
      <div className="Technology">
        <p>SounDark was created in Unity and uses the Oculus Rift as its VR-headset.</p>
      </div>
)


const Design = (props) => (
      <div className="Design">
      <p>Although SounDark managed to entertain many players, early testing proved that most were not comfortable making sounds in order to interact with the game. This cannot be easily solved as it has little to do with game implementation and more to do with culture and societal norms.</p>
      <p>The way we handled this was by making the doors in the maze into a mini-game that could not be completed without sound. At the same time</p>
      <h2 id="challenges">Challenges</h2>
      <p>Simulating soundwaves can be a daunting task. Two viable solutions were available a more physically correct mesh simulation or a simple ray-tracing approach. In the end we opted for ray-tracing as it provided good enough results.</p>
      </div>
)

const Testimonials = (props) => (
  <div className="Testimonials">
    <blockquote>
    <p>It&#39;s like being Daredevil... on crack.</p>
    <p className="name">Marcus, 24</p>
    </blockquote>
    <blockquote>
    <p>It is fun to play, it is scary and it has lots of potential.</p>
    <p className="name">Linn, 23</p>
    </blockquote>
    <blockquote>
    <p>You were supposed to talk but I forgot.</p>
    <p className="name">Björn, 25</p>
    </blockquote>
    <blockquote>
    <p>It was exciting and you felt a bit uneasy. At first you talk and look around, but then you realize you can find out were threats are just by listening.</p>
    <p className="name">David, 26</p>
    </blockquote>
    <blockquote>
    <p>It was fun because it was dark and you could see trough walls.</p>
    <p className="name">Isabel, 23</p>
    </blockquote>
    <blockquote>
    <p>It was entertaining as a thriller because you were both harmless and vulnerable. Escape was the only option. You quite are literally a chicken.</p>
    <p className="name">Marcus, 23</p>
    </blockquote>
    <blockquote>
    <p>It was scary because you get so immersed. I WAS THE CHICKEN.</p>
    <p className="name">Simone, 25</p>
    </blockquote>
    <blockquote>
    <p>Sound is how you see, but also how you get noticed. My strategy? Keep quiet.</p>
    <p className="name">Emma, 23</p>
    </blockquote>
    <blockquote>
    <p>I could imagine this becoming commercially viable. I would gladly pay a couple bucks for this!</p>
    <p className="name">Ewoud, 27</p>
    </blockquote>
  </div>
)
export {Introduction, Technology, Design, Testimonials};
