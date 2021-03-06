import * as React from 'react';
import { Typeahead, TypeaheadModel, TypeaheadProps } from 'react-bootstrap-typeahead';
import { Form } from 'react-bootstrap';
import { getIn, useFormikContext } from 'formik';
import styled from 'styled-components';

interface ITypeaheadFieldProps<T extends TypeaheadModel> extends TypeaheadProps<T> {
  name: string;
  label?: string;
  required?: boolean;
  /** whether or not this component is being used to filter so we can ignore the validation checkmark */
  filter?: boolean;
}

const Group = styled(Form.Group)`
  div {
    width: 100%;
  }
`;

const Label = styled(Form.Label)`
  margin-bottom: 0px;
`;

const Feedback = styled(Form.Control.Feedback)`
  display: block;
`;

export function TypeaheadField<T extends TypeaheadModel>({
  label,
  required,
  name,
  filter,
  ...rest
}: ITypeaheadFieldProps<T>) {
  const { touched, values, errors, setFieldTouched, setFieldValue } = useFormikContext();
  const hasError = !!getIn(touched, name) && !!getIn(errors, name);
  const isValid = !!getIn(touched, name) && !getIn(errors, name);

  return (
    <Group>
      {!!label && (
        <Label>
          {label} {!!required && <span className="required">*</span>}
        </Label>
      )}

      <Typeahead<T>
        {...rest}
        inputProps={{ ...rest.inputProps, name: name }}
        isInvalid={hasError as any}
        isValid={!filter && isValid}
        selected={!!getIn(values, name) ? [getIn(values, name)] : []}
        onChange={(newValues: T[]) => {
          setFieldValue(name, newValues[0]);
        }}
        onBlur={() => setFieldTouched(name, true)}
        id={`${name}-field`}
      />
      {hasError && <Feedback type="invalid">{getIn(errors, name)}</Feedback>}
    </Group>
  );
}
