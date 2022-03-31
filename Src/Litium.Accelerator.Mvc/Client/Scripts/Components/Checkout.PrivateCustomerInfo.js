import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import constants from '../constants';
import { translate } from '../Services/translation';
import { setCountry, setSignUp } from '../Actions/Checkout.action';

const CheckoutPrivateCustomerInfo = ({ onChange }) => {
    const dispatch = useDispatch();
    const { payload, errors = {} } = useSelector((state) => state.checkout);
    const {
        authenticated,
        customerDetails = {},
        alternativeAddress = {},
        signUp,
    } = payload;

    const input = (
        cssClass,
        stateKey,
        id,
        autoComplete = 'on',
        placeholder = null,
        type = 'text',
        maxLength = 200
    ) => (
        <div className={cssClass}>
            <label className="form__label" htmlFor={`${stateKey}-${id}`}>
                {translate(`checkout.customerinfo.${id.toLowerCase()}`)}
                &#8203;
            </label>
            <input
                className="form__input"
                id={`${stateKey}-${id}`}
                name={`${stateKey}-${id}`}
                type={type}
                value={(payload[stateKey] || {})[id] || ''}
                placeholder={placeholder}
                autoComplete={autoComplete}
                onChange={(event) => onChange(stateKey, id, event.target.value)}
                maxLength={maxLength}
            />
            {errors[`${stateKey}-${id}`] && (
                <span
                    className="form__validator--error form__validator--top-narrow"
                    data-error-for={`${stateKey}-${id}`}
                >
                    {errors[`${stateKey}-${id}`][0]}
                </span>
            )}
        </div>
    );
    return (
        <div className="row checkout-info__container">
            <div className="small-12 medium-6 columns">
                <div className="row-inner">
                    <div className="small-12 columns checkout-info__placeholder-container"></div>
                </div>
                <div className="row-inner">
                    {input(
                        'small-6 columns',
                        'customerDetails',
                        'firstName',
                        'billing given-name'
                    )}
                    {input(
                        'small-6 columns',
                        'customerDetails',
                        'lastName',
                        'billing family-name'
                    )}
                    {input(
                        'small-12 columns',
                        'customerDetails',
                        'careOf',
                        'on',
                        null,
                        'text',
                        100
                    )}
                    {input(
                        'small-12 columns',
                        'customerDetails',
                        'address',
                        'billing street-address'
                    )}
                    {input(
                        'small-6 columns',
                        'customerDetails',
                        'zipCode',
                        'billing postal-code',
                        null,
                        'text',
                        50
                    )}
                    {input(
                        'small-6 columns',
                        'customerDetails',
                        'city',
                        'billing address-level2',
                        null,
                        'text',
                        100
                    )}
                    <div className="small-12 columns">
                        <label className="form__label" htmlFor="country">
                            {translate('checkout.customerinfo.country')}
                        </label>
                        <select
                            className="form__input"
                            id="country"
                            value={customerDetails.country}
                            onChange={(event) => {
                                onChange(
                                    'customerDetails',
                                    'country',
                                    event.target.value
                                );
                                onChange(
                                    'alternativeAddress',
                                    'country',
                                    event.target.value
                                );
                                dispatch(setCountry(event.target.value));
                            }}
                        >
                            <option value="" disabled>
                                {translate(
                                    'checkout.customerinfo.country.placeholder'
                                )}
                            </option>
                            {constants.countries &&
                                constants.countries.map(({ text, value }) => (
                                    <option
                                        value={value}
                                        key={`country-${value}`}
                                    >
                                        {text}
                                    </option>
                                ))}
                        </select>
                    </div>
                    {input(
                        'small-12 columns',
                        'customerDetails',
                        'phoneNumber',
                        'billing tel'
                    )}
                </div>
            </div>
            <div className="small-12 medium-6 columns">
                <div className="row-inner">
                    <div className="small-12 columns">
                        <input
                            className="checkout-info__checkbox-input"
                            type="checkbox"
                            id="showAlternativeAddress"
                            name="showAlternativeAddress"
                            checked={
                                alternativeAddress.showAlternativeAddress ||
                                false
                            }
                            onChange={(event) =>
                                onChange(
                                    'alternativeAddress',
                                    'showAlternativeAddress',
                                    event.target.checked
                                )
                            }
                        />
                        <label
                            className="checkout-info__checkbox-label"
                            htmlFor="showAlternativeAddress"
                        >
                            {translate(
                                'checkout.customerinfo.showAlternativeAddress'
                            )}
                        </label>
                    </div>
                </div>
                {alternativeAddress.showAlternativeAddress && (
                    <div className="row-inner">
                        {input(
                            'small-6 columns',
                            'alternativeAddress',
                            'firstName',
                            'shipping given-name'
                        )}
                        {input(
                            'small-6 columns',
                            'alternativeAddress',
                            'lastName',
                            'shipping family-name'
                        )}
                        {input(
                            'small-12 columns',
                            'alternativeAddress',
                            'careOf',
                            'on',
                            null,
                            'text',
                            100
                        )}
                        {input(
                            'small-12 columns',
                            'alternativeAddress',
                            'address',
                            'shipping street-address'
                        )}
                        {input(
                            'small-6 columns',
                            'alternativeAddress',
                            'zipCode',
                            'shipping postal-code',
                            null,
                            'text',
                            50
                        )}
                        {input(
                            'small-6 columns',
                            'alternativeAddress',
                            'city',
                            'shipping address-level2',
                            null,
                            'text',
                            100
                        )}
                        <div className="small-12 columns">
                            <label className="form__label" htmlFor="country2">
                                {translate('checkout.customerinfo.country')}
                            </label>
                            <select
                                className="form__input"
                                id="country2"
                                value={customerDetails.country}
                                onChange={(event) => {
                                    onChange(
                                        'alternativeAddress',
                                        'country',
                                        event.target.value
                                    );
                                    onChange(
                                        'customerDetails',
                                        'country',
                                        event.target.value
                                    );
                                    dispatch(setCountry(event.target.value));
                                }}
                            >
                                <option value="" disabled>
                                    {translate(
                                        'checkout.customerinfo.country.placeholder'
                                    )}
                                </option>
                                {constants.countries &&
                                    constants.countries.map(
                                        ({ text, value }) => (
                                            <option
                                                value={value}
                                                key={`country2-${value}`}
                                            >
                                                {text}
                                            </option>
                                        )
                                    )}
                            </select>
                        </div>
                        {input(
                            'small-12 columns',
                            'alternativeAddress',
                            'phoneNumber',
                            'shipping tel'
                        )}
                    </div>
                )}
            </div>
            <div className="small-12 medium-6 columns">
                <div className="row-inner">
                    {input(
                        'small-12 columns',
                        'customerDetails',
                        'email',
                        'email'
                    )}
                </div>
            </div>
            {!authenticated && (
                <div className="small-12 columns">
                    <div className="row-inner">
                        <div className="small-12 columns">
                            <input
                                className="checkout-info__checkbox-input"
                                type="checkbox"
                                id="signupandlogin"
                                checked={signUp}
                                onChange={(event) =>
                                    dispatch(setSignUp(event.target.checked))
                                }
                            />
                            <label
                                className="checkout-info__checkbox-label"
                                htmlFor="signupandlogin"
                            >
                                {translate(
                                    'checkout.customerinfo.signupandlogin'
                                )}
                            </label>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default CheckoutPrivateCustomerInfo;
