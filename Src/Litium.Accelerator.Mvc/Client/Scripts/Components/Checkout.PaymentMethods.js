import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { translate } from '../Services/translation';
import {
    setDiscountCode,
    submitDiscountCode,
    setPayment,
    deleteDiscountCode,
} from '../Actions/Checkout.action';

const CheckoutPaymentMethods = () => {
    const dispatch = useDispatch();
    const { payload, errors = {} } = useSelector((state) => state.checkout);
    const {
        paymentMethods,
        selectedPaymentMethod,
        usedDiscountCodes,
        discountCode,
    } = payload;

    return (
        <section className="row checkout-info__container">
            <div className="columns small-12">
                {paymentMethods &&
                    paymentMethods.map((method) => (
                        <label className="row no-margin" key={method.id}>
                            <input
                                type="radio"
                                name="paymentMethods"
                                className="checkout-info__checkbox-radio"
                                value={method.id}
                                checked={method.id === selectedPaymentMethod}
                                onChange={() => dispatch(setPayment(method.id))}
                            />
                            <span className="columns">
                                <b> {method.name} </b> - {method.formattedPrice}
                            </span>
                        </label>
                    ))}
            </div>
        </section>
    );
};

export default CheckoutPaymentMethods;
