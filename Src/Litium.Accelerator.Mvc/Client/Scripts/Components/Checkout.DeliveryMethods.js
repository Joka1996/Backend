import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { setDelivery } from '../Actions/Checkout.action';

const CheckoutDeliveryMethods = () => {
    const dispatch = useDispatch();
    const { deliveryMethods, selectedDeliveryMethod } = useSelector(
        (state) => state.checkout.payload
    );

    return (
        deliveryMethods &&
        deliveryMethods.length > 0 && (
            <section className="row checkout-info__container">
                <div className="columns small-12">
                    {deliveryMethods.map((method) => (
                        <label className="row no-margin" key={method.id}>
                            <input
                                type="radio"
                                name="deliveryMethods"
                                className="checkout-info__checkbox-radio"
                                value={method.id}
                                checked={method.id === selectedDeliveryMethod}
                                onChange={() =>
                                    dispatch(setDelivery(method.id))
                                }
                            />
                            <span className="columns">
                                <b> {method.name} </b> - {method.formattedPrice}
                            </span>
                        </label>
                    ))}
                </div>
            </section>
        )
    );
};

export default CheckoutDeliveryMethods;
