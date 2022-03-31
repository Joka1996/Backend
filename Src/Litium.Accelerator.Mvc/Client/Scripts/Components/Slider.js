import React from 'react';
import 'react-responsive-carousel/lib/styles/carousel.min.css';
import { Carousel } from 'react-responsive-carousel';

const CarouselSettings = {
    showStatus: false,
    showThumbs: false,
    infiniteLoop: true,
};

const Slider = ({ values }) => (
    <Carousel {...CarouselSettings}>
        {values.map((value, index) => (
            <div key={`figure${index}`}>
                <a className="slider__link" href={value.url}>
                    <img className="slider__image" src={value.image} />
                </a>
                <div className="banner-text">
                    <h3 className="banner-text__title">{value.text}</h3>
                    {value.actionText && value.actionText.length > 0 && (
                        <span className="banner-text__button">
                            {value.actionText}
                        </span>
                    )}
                </div>
            </div>
        ))}
    </Carousel>
);

export default Slider;
